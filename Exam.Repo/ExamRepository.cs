using Microsoft.EntityFrameworkCore;
using Exam.Data;
using Exam.Models;
using ExamEntity = Exam.Models.Exam;  

namespace Exam.Repo
{
    public class ExamRepository : IExamRepository
    {
        private readonly ApiContext _context;

        public ExamRepository(ApiContext context) => _context = context;

        public async Task AddAsync(ExamEntity exam)
        {
            await _context.Exams.AddAsync(exam);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ExamEntity>> GetAllAsync() =>
            await _context.Exams
                .Include(e => e.ExamQuestions)
                .ToListAsync();

        public async Task<ExamEntity?> GetByIdAsync(int id) =>
            await _context.Exams
                .Include(e => e.ExamQuestions)
                .FirstOrDefaultAsync(e => e.Id == id);

        public async Task<ExamEntity?> GetWithQuestionsAndChoicesAsync(int id) =>
            await _context.Exams
                .Include(e => e.ExamQuestions)
                    .ThenInclude(eq => eq.Question)
                        .ThenInclude(q => q!.Choices)
                .FirstOrDefaultAsync(e => e.Id == id);

        public async Task UpdateAsync(ExamEntity exam)
        {
            _context.Exams.Update(exam);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ExamEntity exam)
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

        public async Task<bool> ExistsAsync(int id) =>
            await _context.Exams.AnyAsync(e => e.Id == id);
    }
}