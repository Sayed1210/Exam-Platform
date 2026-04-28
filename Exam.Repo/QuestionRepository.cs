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

        public async Task<Question> CreateAsync(Question question)
        {
            _db.Questions.Add(question);
            await _db.SaveChangesAsync();
            return question;
        }

        public async Task<Question> UpdateAsync(Question question)
        {
            _db.Questions.Update(question);
            await _db.SaveChangesAsync();
            return question;
        }

        public async Task DeleteAsync(Question question)
        {
            _db.Questions.Remove(question);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id) =>
            await _db.Questions.AnyAsync(q => q.Id == id);
    }
}