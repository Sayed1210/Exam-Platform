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

        public async Task<(IEnumerable<Question> Items, int TotalCount)> GetPagedAsync(int page, int pageSize)
        {
            var totalCount = await _db.Questions.CountAsync();

            var items = await _db.Questions
                .Include(q => q.Topic)
                .Include(q => q.Choices)
                .AsNoTracking()
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