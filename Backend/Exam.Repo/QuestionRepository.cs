using Microsoft.EntityFrameworkCore;
using Exam.Models;
using Exam.Data;

namespace Exam.Repo
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly ApiContext _db;

        public QuestionRepository(ApiContext db) => _db = db;

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

        public async Task<Question> CreateAsync(Question question)
        {
            _db.Questions.Add(question);
            await _db.SaveChangesAsync();
            // Reload with choices included
            return await _db.Questions
                .Include(q => q.Topic)
                .Include(q => q.Choices)
                .FirstAsync(q => q.Id == question.Id);
        }


        public async Task<Question> UpdateAsync(Question question, bool updateChoices = false)
        {
            var existing = await _db.Questions
                .Include(q => q.Choices)
                .FirstAsync(q => q.Id == question.Id);

            // Only update text if provided
            if (question.Text is not null)
                existing.Text = question.Text;

            // Only update imageUrl if provided
            if (question.ImageUrl is not null)
                existing.ImageUrl = question.ImageUrl;

            // Only touch choices if explicitly requested
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
        public async Task DeleteAsync(Question question)
        {
            _db.Questions.Remove(question);
            await _db.SaveChangesAsync();
        }

public async Task<(IEnumerable<Question> items, int totalCount)> GetPagedAsync(
    int page, int pageSize, string? search = null, int? topicId = null)
{
    var query = _db.Questions
        .Include(q => q.Topic)
        .Include(q => q.Choices)
        .AsNoTracking()
        .AsQueryable();

    if (!string.IsNullOrEmpty(search))
        query = query.Where(q => q.Text.Contains(search));

    if (topicId.HasValue)
        query = query.Where(q => q.TopicId == topicId.Value);

    var totalCount = await query.CountAsync();

    var items = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return (items, totalCount);
}
        public async Task<Question> UpdateChoiceAsync(Choice choice)
        {
            _db.Choices.Update(choice);
            await _db.SaveChangesAsync();

            return await _db.Questions
                .Include(q => q.Topic)
                .Include(q => q.Choices)
                .FirstAsync(q => q.Id == choice.QuestionId);
        }
        public async Task<bool> ExistsAsync(int id) =>
            await _db.Questions.AnyAsync(q => q.Id == id);
    }
}