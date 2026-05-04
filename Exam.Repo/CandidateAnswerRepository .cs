using Microsoft.EntityFrameworkCore;
namespace Exam.Repo;
using Exam.Models;
using Exam.Data;   

public class CandidateAnswerRepository(ApiContext context) : ICandidateAnswerRepository
{
    private readonly ApiContext _context = context;

    public async Task AddRangeAsync(List<CandidateAnswer> answers)
    {
        // Adds multiple CandidateAnswer entities to EF Core change tracker
        // & immediately persists them to the database in a single SaveChanges call
        _context.CandidateAnswers.AddRange(answers);
        await _context.SaveChangesAsync();
    }

    public async Task<List<int>> GetCorrectChoiceIdsAsync(List<int> questionIds)
    {
        return await _context.Choices
            .Where(c => questionIds.Contains(c.QuestionId) && c.IsCorrect)
            .Select(c => c.Id)
            .ToListAsync();
    }

}