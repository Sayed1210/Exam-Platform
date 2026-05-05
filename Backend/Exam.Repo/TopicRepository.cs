using Microsoft.EntityFrameworkCore;
using Exam.Models;
using Exam.Data;

namespace Exam.Repo
{
    public class TopicRepository : ITopicRepository
    {
        private readonly ApiContext _context;

        public TopicRepository(ApiContext context) => _context = context;

        public async Task AddAsync(Topic topic)
        {
            await _context.Topics.AddAsync(topic);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Topic>> GetAllAsync() =>
            await _context.Topics
                .Include(t => t.Questions)
                .AsNoTracking()
                .ToListAsync();

        public async Task<Topic?> GetByIdAsync(int id) =>
            await _context.Topics
                .Include(t => t.Questions)
                .FirstOrDefaultAsync(t => t.Id == id);

        public async Task UpdateAsync(Topic topic)
        {
            _context.Topics.Update(topic);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Topic topic)
        {
            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id) =>
            await _context.Topics.AnyAsync(t => t.Id == id);

        public async Task<bool> TitleExistsAsync(string title, int? excludeId = null) =>
            await _context.Topics
                .AnyAsync(t => t.Title.ToLower() == title.ToLower()
                            && (!excludeId.HasValue || t.Id != excludeId.Value));
    }
}