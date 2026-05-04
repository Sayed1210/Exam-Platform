using Exam.Models;
using ExamEntity = Exam.Models.Exam;

namespace Exam.Repo
{
    public interface IExamRepository
    {
        Task AddAsync(ExamEntity exam);
        Task<IEnumerable<ExamEntity>> GetAllAsync();
        Task<ExamEntity?> GetByIdAsync(int id);

        Task<ExamEntity?> GetWithQuestionsAndChoicesAsync(int id);
        Task UpdateAsync(ExamEntity exam, List<int>? questionIds);
        Task DeleteAsync(ExamEntity exam);
        Task AssignQuestionsAsync(List<ExamQuestion> examQuestions);
        Task RemoveQuestionAsync(int examId, int questionId);
        Task<IEnumerable<ExamEntity>> GetAllWithQuestionsAndChoicesAsync();
        Task<bool> ExistsAsync(int id);
    }
}