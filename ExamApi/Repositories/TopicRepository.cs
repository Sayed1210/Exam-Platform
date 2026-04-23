using Microsoft.EntityFrameworkCore;

namespace ExamApi.Repositories
{
    public class TopicRepository : ITopicRepository
    {
        private readonly ApiContext _context;

        public TopicRepository(ApiContext context)
        {
            _context = context;
        }

        // CREATE
    
        public async Task AddAsync(Topic topic)
        {
            await _context.Topics.AddAsync(topic);
            await _context.SaveChangesAsync();
        }

        // READ - Get All
        
        public async Task<IEnumerable<Topic>> GetAllAsync()
        {
            return await _context.Topics
                .Include(t => t.Questions)
                .AsNoTracking()
                .ToListAsync();
        }

        // READ - Get By Id
        
        public async Task<Topic?> GetByIdAsync(int id)
        {
            return await _context.Topics
                .Include(t => t.Questions)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

       
        // UPDATE
    
        public async Task UpdateAsync(Topic topic)
        {
            _context.Topics.Update(topic);
            await _context.SaveChangesAsync();
        }

        
        // DELETE
        
        public async Task DeleteAsync(Topic topic)
        {
            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();
        }

     
        // HELPER - Check if exists
        
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Topics
                .AnyAsync(t => t.Id == id);
        }

        
        // HELPER - Check title is unique
        
        public async Task<bool> TitleExistsAsync(string title, int? excludeId = null)
        {
            return await _context.Topics
                .AnyAsync(t => t.Title.ToLower() == title.ToLower()
                            && (!excludeId.HasValue || t.Id != excludeId.Value));
        }
    }
}
