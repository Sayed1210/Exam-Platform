using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Exam.Data;
using Exam.Models;
namespace Exam.Repo;
public class CandidateExamRepository(ApiContext context) : ICandidateExamRepository
{
    private readonly ApiContext _context = context;
    public async Task<Candidate?> GetCandidateByEmailAsync(string email)
    {
        return await _context.Candidates.AsNoTracking().FirstOrDefaultAsync(c => c.Email == email);
    }

    public async Task AddInvitationAsync(CandidateExam invitation)
    {
        await _context.CandidateExams.AddAsync(invitation);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }

    public async Task<CandidateExam?> GetAsync(int candidateId, int examId)
    {
        // Get a CandidateExam from DB by [CandidateId + ExamId]
        return await _context.CandidateExams
            .FirstOrDefaultAsync(x => x.CandidateId == candidateId && x.ExamId == examId);
    }

    public async Task SaveAsync(CandidateExam candidateExam)
    {
        // Save all pending changes tracked by EF Core to db in one transaction
        // await _context.SaveChangesAsync();
        _context.CandidateExams.Update(candidateExam);
        await _context.SaveChangesAsync();
    }

    public async Task<CandidateExam?> GetByInvitationTokenAsync(string token)
    {
        return await _context.CandidateExams
            .Include(ce => ce.Candidate)
            .FirstOrDefaultAsync(ce => ce.InvitationToken == token);
    }

}
