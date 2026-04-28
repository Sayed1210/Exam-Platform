using Exam.Models.dtos.requests;
using Exam.Models.dtos.responses;

namespace Exam.Service
{
    public interface IExamService
    {
        // Create
        Task<ExamResponseDto> CreateExamAsync(CreateExamDto dto);

        // Read
        Task<IEnumerable<ExamResponseDto>> GetAllExamsAsync();
        Task<ExamResponseDto?> GetExamByIdAsync(int id);

        Task<ExamResponseDto?> GetExamWithQuestionsAsync(int id);

        // Update
        Task<ExamResponseDto?> UpdateExamAsync(int id, CreateExamDto dto);

        Task<ExamResponseDto> AssignQuestionsAsync(int examId, List<int> questionIds);
        Task<ExamResponseDto> RemoveQuestionAsync(int examId, int questionId);

        // Delete
        Task<bool> DeleteExamAsync(int id);
    }
}
