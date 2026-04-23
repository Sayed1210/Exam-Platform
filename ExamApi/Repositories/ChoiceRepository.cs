using Microsoft.EntityFrameworkCore;
namespace ExamApi.Repositories
{
    public class ChoiceRepository : IChoiceRepository
    {
        private readonly ApiContext _db;

        public ChoiceRepository(ApiContext db) => _db = db;

        public async Task<IEnumerable<Choice>> GetAllByQuestionIdAsync(int questionId) =>
            await _db.Choices
                .Where(c => c.QuestionId == questionId)
                .AsNoTracking()
                .ToListAsync();

        public async Task<Choice?> GetByIdAsync(int id) =>
            await _db.Choices
                .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<Choice> CreateAsync(Choice choice)
        {
            _db.Choices.Add(choice);
            await _db.SaveChangesAsync();
            return choice;
        }

        public async Task<Choice> UpdateAsync(Choice choice)
        {
            _db.Choices.Update(choice);
            await _db.SaveChangesAsync();
            return choice;
        }

        public async Task DeleteAsync(Choice choice)
        {
            _db.Choices.Remove(choice);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id) =>
            await _db.Choices.AnyAsync(c => c.Id == id);
    }
}
