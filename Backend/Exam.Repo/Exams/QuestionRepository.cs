using Microsoft.EntityFrameworkCore;
using Exam.Models;
using Exam.Data;
using Microsoft.Extensions.Logging;

namespace Exam.Repo
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly ApiContext _db;
        private readonly ILogger<QuestionRepository> _logger;

        public QuestionRepository(ApiContext db, ILogger<QuestionRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IEnumerable<Question>> GetAllAsync() =>
            await _db.Questions
                .Include(q => q.Topic)
                .Include(q => q.Choices)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<Question>> GetByTopicIdAsync(int topicId) =>
            await _db.Questions
                .Where(q => q.TopicId == topicId)
                .Include(q => q.Topic)
                .Include(q => q.Choices)
                .AsNoTracking()
                .ToListAsync();

        public async Task<Question?> GetByIdAsync(int id) =>
            await _db.Questions
                .Include(q => q.Topic)
                .Include(q => q.Choices)
                .FirstOrDefaultAsync(q => q.Id == id);

        public async Task<List<int>> GetExistingIdsAsync(List<int> ids) =>
            await _db.Questions
                .Where(q => ids.Contains(q.Id))
                .Select(q => q.Id)
                .ToListAsync();

        public async Task<(IEnumerable<Question> items, int totalCount)> GetPagedAsync(
            int page, int pageSize, string? search = null, int[]? topicIds = null)
        {
            var query = _db.Questions
                .Include(q => q.Topic)
                .Include(q => q.Choices)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(q => q.Text.Contains(search));

            if (topicIds is not null && topicIds.Length > 0)
                query = query.Where(q => topicIds.Contains(q.TopicId));

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<bool> ExistsAsync(int id) =>
            await _db.Questions.AnyAsync(q => q.Id == id);

        public async Task<Question> CreateAsync(Question question)
        {
            try
            {
                _db.Questions.Add(question);
                await _db.SaveChangesAsync();
                return await _db.Questions
                    .Include(q => q.Topic)
                    .Include(q => q.Choices)
                    .FirstAsync(q => q.Id == question.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateAsync failed — TopicId={TopicId}", question.TopicId);
                return new Question();
            }
        }

        public async Task<Question> UpdateAsync(Question question, bool updateChoices = false)
        {
            try
            {
                var existing = await _db.Questions
                    .Include(q => q.Choices)
                    .FirstAsync(q => q.Id == question.Id);

                if (question.Text is not null)
                    existing.Text = question.Text;

                if (question.ImageUrl is not null)
                    existing.ImageUrl = question.ImageUrl;

                if (updateChoices)
                {
                    var oldChoices = await _db.Choices
                        .Where(c => c.QuestionId == question.Id)
                        .ToListAsync();
                    _db.Choices.RemoveRange(oldChoices);
                    existing.Choices = question.Choices ?? [];
                }

                await _db.SaveChangesAsync();

                return await _db.Questions
                    .Include(q => q.Topic)
                    .Include(q => q.Choices)
                    .FirstAsync(q => q.Id == question.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync failed — QuestionId={QuestionId}", question.Id);
                return new Question();
            }
        }

        public async Task<Question> UpdateChoiceAsync(Choice choice)
        {
            try
            {
                _db.Choices.Update(choice);
                await _db.SaveChangesAsync();

                return await _db.Questions
                    .Include(q => q.Topic)
                    .Include(q => q.Choices)
                    .FirstAsync(q => q.Id == choice.QuestionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "UpdateChoiceAsync failed — ChoiceId={ChoiceId} QuestionId={QuestionId}",
                    choice.Id, choice.QuestionId);
                return new Question();
            }
        }

        public async Task DeleteAsync(Question question)
        {
            try
            {
                _db.Questions.Remove(question);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync failed — QuestionId={QuestionId}", question.Id);
            }
        }
    }
}