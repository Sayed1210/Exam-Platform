using Exam.Models;

namespace Exam.Service
{
    public interface IExamService
    {
        // Create
        Task<ExamResponse> CreateExamAsync(CreateExamRequest dto);

        // Read
        Task<IEnumerable<ExamResponse>> GetAllExamsAsync();
        Task<ExamResponse?> GetExamByIdAsync(int id);
        Task<IEnumerable<ExamResponse>> GetAllExamsWithQuestionsAsync();

        Task<ExamResponse?> GetExamWithQuestionsAsync(int id);

        // Update
        Task<ExamResponse?> UpdateExamAsync(int id, UpdateExamRequest dto);

        Task<ExamResponse> AssignQuestionsAsync(int examId, List<int> questionIds);
        Task<ExamResponse?> RemoveQuestionAsync(int examId, int questionId);

        // Delete
        Task<bool> DeleteExamAsync(int id);
    }
}
