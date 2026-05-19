using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Exam.Models;
using Exam.Data;

namespace Exam.Repo;

public class CandidateAnswerRepository(
    ApiContext context,
    ILogger<CandidateAnswerRepository> logger
) : ICandidateAnswerRepository
{
    private readonly ApiContext _context = context;
    private readonly ILogger<CandidateAnswerRepository> _logger = logger;

    public async Task AddRangeAsync(List<CandidateAnswer> answers)
    {
        try
        {
            _context.CandidateAnswers.AddRange(answers);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "AddRangeAsync failed — AnswersCount={AnswersCount}",
                answers.Count);
        }
    }

    public async Task<List<int>> GetCorrectChoiceIdsAsync(List<int> questionIds)
    {
        return await _context.Choices
            .Where(c =>
                questionIds.Contains(c.QuestionId) &&
                c.IsCorrect)
            .Select(c => c.Id)
            .ToListAsync();
    }
}