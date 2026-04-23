using ExamApi.DTOs.Requests;
using ExamApi.DTOs.Responses;

namespace ExamApi.Services
{
    public interface IQuestionService
    {
        Task<IEnumerable<QuestionResponse>> GetAllAsync();
        Task<IEnumerable<QuestionResponse>> GetByTopicIdAsync(int topicId);
        Task<QuestionResponse?> GetByIdAsync(int id);
        Task<QuestionResponse> CreateAsync(QuestionRequest request);
        Task<QuestionResponse?> UpdateAsync(int id, QuestionRequest request);
        Task<bool> DeleteAsync(int id);
    }

}
