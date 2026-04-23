
using Microsoft.EntityFrameworkCore;
namespace ExamApi.Repositories
{
    public class ExamRepository : IExamRepository
    {
        private readonly ApiContext _context;

        public ExamRepository(ApiContext context)
        {
            _context = context;
        }

        // CREATE
        public async Task AddAsync(Exam exam)
        {
            await _context.Exams.AddAsync(exam);
            await _context.SaveChangesAsync();
        }

        // READ - Get All
        public async Task<IEnumerable<Exam>> GetAllAsync()
        {
            return await _context.Exams.ToListAsync();
        }

        // READ - Get By Id
        public async Task<Exam?> GetByIdAsync(int id)
        {
            return await _context.Exams.FindAsync(id);
        }

        // UPDATE
        public async Task UpdateAsync(Exam exam)
        {
            _context.Exams.Update(exam);
            await _context.SaveChangesAsync();
        }

        // DELETE
        public async Task DeleteAsync(Exam exam)
        {
            _context.Exams.Remove(exam);
            await _context.SaveChangesAsync();
        }
      

      

        // HELPER - Check if exists
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Exams.AnyAsync(e => e.Id == id);
        }
    }
}
