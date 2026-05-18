namespace Exam.Repo;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Exam.Data;
using Exam.Models;


public class CandidateRepository(ApiContext context) : ICandidateRepository
{
    // Repository needs DbContext to access the database (DI)
    private readonly ApiContext _context = context;

public async Task<List<Candidate>> GetAllAsync()
{
    return await _context.Candidates
        .Include(c => c.CandidateExams)
        .ToListAsync();
}
    public async Task<Candidate?> GetByIdAsync(int id)
    {
        // SELECT * FROM Candidates WHERE Id = @id LIMIT 1
        return await _context.Candidates
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Candidate?> GetByEmailAsync(string email)
    {
        return await _context.Candidates
            .FirstOrDefaultAsync(c => c.Email == email);
    }

    public async Task<Candidate?> GetCandidateByExamIdAsync(int examId)
    {
        // Candidates WHERE EXISTS (CandidateExams.ExamId == examId)
        return await _context.Candidates
            .Include(c => c.CandidateExams)
            .FirstOrDefaultAsync(c => c.CandidateExams
                .Any(ce => ce.ExamId == examId));
    }
public async Task<int> CountAsync(string? search, int? status, bool noStatus = false)
{
    var query = _context.Candidates
        .Include(c => c.CandidateExams)
        .AsQueryable();

    if (!string.IsNullOrEmpty(search))
        query = query.Where(c =>
            c.FirstName.Contains(search) ||
            c.LastName.Contains(search) ||
            c.Email.Contains(search));

    if (noStatus)
        query = query.Where(c => !c.CandidateExams.Any());
else if (status.HasValue)
{


     string statusString = ((ExamStatus)status.Value).ToString();
    query = query.Where(c => c.CandidateExams
        .Any(e => e.Status.ToString() == statusString));
}

    return await query.CountAsync();
}

public async Task<List<Candidate>> GetPagedAsync(
    int page, int pageSize, string? search, int? status, bool noStatus = false)
{
    Console.WriteLine($"DEBUG: status={status}, noStatus={noStatus}");
    var query = _context.Candidates
        .Include(c => c.CandidateExams)
        .AsQueryable();

    if (!string.IsNullOrEmpty(search))
        query = query.Where(c =>
            c.FirstName.Contains(search) ||
            c.LastName.Contains(search) ||
            c.Email.Contains(search));

    if (noStatus)
        query = query.Where(c => !c.CandidateExams.Any());
   else if (status.HasValue)
{


    string statusString = ((ExamStatus)status.Value).ToString();
    query = query.Where(c => c.CandidateExams
        .Any(e => e.Status.ToString() == statusString));
}

    return await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
}
    
    public async Task AddAsync(Candidate candidate)
    {
        // Add entity to EF change tracker (not database yet)
        _context.Candidates.Add(candidate);
        // INSERT happens only when SaveChangesAsync is called
        await _context.SaveChangesAsync();
    }
    public async Task<Candidate?> GetWithExamsAndAnswersAsync(int candidateId) =>
    await _context.Candidates
        .Include(c => c.CandidateExams)
            .ThenInclude(ce => ce.Exam)
        .Include(c => c.CandidateAnswers)
            .ThenInclude(ca => ca.Question)
        .Include(c => c.CandidateAnswers)
            .ThenInclude(ca => ca.Choice)
        .FirstOrDefaultAsync(c => c.Id == candidateId);

    public async Task<List<Candidate>> GetUnassignedCandidatesAsync()
    {
        return await _context.Candidates
            .Where(c => !c.CandidateExams.Any())
            .ToListAsync();
    }

    public async Task DeleteAsync(int id)
    {
        // Find candidate in DB
        var candidate = await _context.Candidates
            .FirstOrDefaultAsync(c => c.Id == id);

        if (candidate == null)
            throw new Exception($"Candidate with id {id} not found");

        // Mark entity for deletion
        _context.Candidates.Remove(candidate);

        // Executes DELETE in DB
        await _context.SaveChangesAsync();

    }
}
