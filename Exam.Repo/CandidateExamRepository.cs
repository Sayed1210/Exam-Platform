namespace Exam.Repositories;
using Microsoft.EntityFrameworkCore;
using Exam.Models;
using Exam.Data;


public class CandidateExamRepository(ApiContext context) : ICandidateExamRepository
{
    private readonly ApiContext _context = context;

    
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
}