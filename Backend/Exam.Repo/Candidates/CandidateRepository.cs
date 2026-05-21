using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Exam.Data;
using Exam.Models;

namespace Exam.Repo;

public class CandidateRepository(
    ApiContext context,
    ILogger<CandidateRepository> logger
) : ICandidateRepository
{
    private readonly ApiContext _context = context;
    private readonly ILogger<CandidateRepository> _logger = logger;

    public async Task<List<Candidate>> GetAllAsync()
    {
        return await _context.Candidates
            .Include(c => c.CandidateExams)
            .ToListAsync();
    }

    public async Task<Candidate?> GetByIdAsync(int id)
    {
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
        return await _context.Candidates
            .Include(c => c.CandidateExams)
            .FirstOrDefaultAsync(c =>
                c.CandidateExams.Any(ce => ce.ExamId == examId));
    }

    public async Task<int> CountAsync(
        string? search,
        int? status,
        bool noStatus = false)
    {
        var query = _context.Candidates
            .Include(c => c.CandidateExams)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(c =>
                c.FirstName.Contains(search) ||
                c.LastName.Contains(search) ||
                c.Email.Contains(search));
        }

        if (noStatus)
        {
            query = query.Where(c => !c.CandidateExams.Any());
        }
        else if (status.HasValue)
        {
            string statusString =
                ((ExamStatus)status.Value).ToString();

            query = query.Where(c =>
                c.CandidateExams.Any(e =>
                    e.Status.ToString() == statusString));
        }

        return await query.CountAsync();
    }

    public async Task<List<Candidate>> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        int? status,
        bool noStatus = false)
    {
        var query = _context.Candidates
            .Include(c => c.CandidateExams)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(c =>
                c.FirstName.Contains(search) ||
                c.LastName.Contains(search) ||
                c.Email.Contains(search));
        }

        if (noStatus)
        {
            query = query.Where(c => !c.CandidateExams.Any());
        }
        else if (status.HasValue)
        {
            string statusString =
                ((ExamStatus)status.Value).ToString();

            query = query.Where(c =>
                c.CandidateExams.Any(e =>
                    e.Status.ToString() == statusString));
        }

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task AddAsync(Candidate candidate)
    {
        try
        {
            _context.Candidates.Add(candidate);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "AddAsync failed — CandidateEmail={Email}",
                candidate.Email);
        }
    }

    public async Task<Candidate?> GetWithExamsAndAnswersAsync(int candidateId)
    {
        return await _context.Candidates
            .AsSplitQuery()
            .Include(c => c.CandidateExams)
                .ThenInclude(ce => ce.Exam)
            .Include(c => c.CandidateAnswers)
                .ThenInclude(ca => ca.Question)
            .Include(c => c.CandidateAnswers)
                .ThenInclude(ca => ca.Choice)
            .FirstOrDefaultAsync(c => c.Id == candidateId);
    }

    public async Task<List<Candidate>> GetUnassignedCandidatesAsync()
    {
        return await _context.Candidates
            .Where(c => !c.CandidateExams.Any())
            .ToListAsync();
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            var candidate = await _context.Candidates
                .FirstOrDefaultAsync(c => c.Id == id);

            if (candidate == null)
                return;

            var candidateExams = _context.CandidateExams
                .Where(ce => ce.CandidateId == id);

            _context.CandidateExams.RemoveRange(candidateExams);

            _context.Candidates.Remove(candidate);

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "DeleteAsync failed — CandidateId={CandidateId}",
                id);
        }
    }
}