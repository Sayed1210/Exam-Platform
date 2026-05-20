using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Exam.Data;
using Exam.Models;

namespace Exam.Repo;

public class CandidateExamRepository(
    ApiContext context,
    ILogger<CandidateExamRepository> logger
) : ICandidateExamRepository
{
    private readonly ApiContext _context = context;
    private readonly ILogger<CandidateExamRepository> _logger = logger;

    public async Task<Candidate?> GetCandidateAsync(int id)
    {
        return await _context.Candidates
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task AddInvitationAsync(CandidateExam invitation)
    {
        try
        {
            await _context.CandidateExams.AddAsync(invitation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "AddInvitationAsync failed — CandidateId={CandidateId}, ExamId={ExamId}",
                invitation.CandidateId,
                invitation.ExamId);
        }
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }

    public async Task<bool> ExamExistsAsync(int examId)
    {
        return await _context.Exams
            .AnyAsync(e => e.Id == examId);
    }


    public async Task<List<int>> GetExamQuestionIdsAsync(int examId)
    {
        return await _context.ExamQuestions
            .Where(q => q.ExamId == examId)
            .Select(q => q.QuestionId)
            .ToListAsync();
    }
    public async Task SaveChangesAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "SaveChangesAsync failed in CandidateExamRepository");
        }
    }

    public async Task<CandidateExam?> GetAsync(int candidateId, int examId)
    {
        return await _context.CandidateExams
            .FirstOrDefaultAsync(x =>
                x.CandidateId == candidateId &&
                x.ExamId == examId);
    }

    public async Task SaveAsync(CandidateExam candidateExam)
    {
        try
        {
            _context.CandidateExams.Update(candidateExam);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "SaveAsync failed — CandidateId={CandidateId}, ExamId={ExamId}",
                candidateExam.CandidateId,
                candidateExam.ExamId);
        }
    }

    public async Task<CandidateExam?> GetByInvitationTokenAsync(string token)
    {
        return await _context.CandidateExams
            .Include(ce => ce.Candidate)
            .FirstOrDefaultAsync(ce => ce.InvitationToken == token);
    }
}