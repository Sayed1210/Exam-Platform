using Microsoft.EntityFrameworkCore;
using Exam.Models;
using Exam.Data;
using Microsoft.Extensions.Logging;

namespace Exam.Repo
{
    public class TopicRepository : ITopicRepository
    {
        private readonly ApiContext _context;
        private readonly ILogger<TopicRepository> _logger;

        public TopicRepository(ApiContext context, ILogger<TopicRepository> logger)
        {
            _context = context;
            _logger = logger;
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

        public async Task<bool> ExistsAsync(int id) =>
            await _context.Topics.AnyAsync(t => t.Id == id);

        public async Task<bool> TitleExistsAsync(string title, int? excludeId = null) =>
            await _context.Topics
                .AnyAsync(t => t.Title.ToLower() == title.ToLower()
                            && (!excludeId.HasValue || t.Id != excludeId.Value));

        public async Task AddAsync(Topic topic)
        {
            try
            {
                await _context.Topics.AddAsync(topic);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddAsync failed — Title={Title}", topic.Title);
            }
        }

        public async Task UpdateAsync(Topic topic)
        {
            try
            {
                _context.Topics.Update(topic);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync failed — TopicId={TopicId}", topic.Id);
            }
        }

        public async Task DeleteAsync(Topic topic)
        {
            try
            {
                _context.Topics.Remove(topic);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync failed — TopicId={TopicId}", topic.Id);
            }
        }
    }
}