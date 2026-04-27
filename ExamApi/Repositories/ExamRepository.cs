
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
        public async Task<Exam?> GetByIdAsync(int id) =>
      await _context.Exams
          .Include(e => e.ExamQuestions)
          .FirstOrDefaultAsync(e => e.Id == id);


        // READ - Get with Questions and Choices
        public async Task<Exam?> GetWithQuestionsAndChoicesAsync(int id) =>
            await _context.Exams
                .Include(e => e.ExamQuestions)
                    .ThenInclude(eq => eq.Question)
                        .ThenInclude(q => q!.Choices)
                .FirstOrDefaultAsync(e => e.Id == id);

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
        public async Task AssignQuestionsAsync(List<ExamQuestion> examQuestions)
        {
            _context.ExamQuestions.AddRange(examQuestions);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveQuestionAsync(int examId, int questionId)
        {
            var eq = await _context.ExamQuestions
                .FirstOrDefaultAsync(eq => eq.ExamId == examId && eq.QuestionId == questionId);

            if (eq is null) return;

            _context.ExamQuestions.Remove(eq);
            await _context.SaveChangesAsync();
        }





        // HELPER - Check if exists
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Exams.AnyAsync(e => e.Id == id);
        }
    }
}
