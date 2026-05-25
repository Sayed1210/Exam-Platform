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

    public async Task<CandidateAnswer?> GetAsync(int candidateId, int examId, int questionId)
    {
        try
        {
            return await _context.CandidateAnswers.FindAsync(candidateId, questionId, examId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "GetAsync failed — CandidateId={CandidateId} ExamId={ExamId} QuestionId={QuestionId}",
                candidateId, examId, questionId);
            return null;
        }
    }

    public async Task<List<CandidateAnswer>> GetByCandidateExamAsync(int candidateId, int examId)
    {
        return await _context.CandidateAnswers
            .AsNoTracking()
            .Where(a => a.CandidateId == candidateId && a.ExamId == examId)
            .ToListAsync();
    }

    public async Task AddOrUpdateAsync(CandidateAnswer answer)
    {
        try
        {
            var existing = await _context.CandidateAnswers.FindAsync(answer.CandidateId, answer.QuestionId, answer.ExamId);
            if (existing is null)
            {
                await _context.CandidateAnswers.AddAsync(answer);
            }
            else
            {
                existing.ChoiceId = answer.ChoiceId;
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "AddOrUpdateAsync failed — CandidateId={CandidateId} ExamId={ExamId} QuestionId={QuestionId}",
                answer.CandidateId, answer.ExamId, answer.QuestionId);
        }
    }

    public async Task<bool> ChoiceBelongsToQuestionAsync(int choiceId, int questionId)
    {
        return await _context.Choices
            .AnyAsync(c => c.Id == choiceId && c.QuestionId == questionId);
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
