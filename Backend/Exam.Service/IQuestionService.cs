using Exam.Models;

namespace Exam.Service
{
    public interface IQuestionService
    {
    Task<PagedResponse<QuestionResponse>> GetAllAsync(
    int page, int pageSize, string? search = null, int? topicId = null);
        Task<IEnumerable<QuestionResponse>> GetByTopicIdAsync(int topicId);
        Task<QuestionResponse?> GetByIdAsync(int id);
        Task<QuestionResponse> CreateAsync(QuestionRequest request);
        
        Task<QuestionResponse?> UpdateAsync(int id, UpdateQuestionRequest request);
        Task<QuestionResponse?> UpdateChoiceAsync(int questionId, int choiceId, ChoiceRequest request);
        Task<bool> DeleteAsync(int id);
    }

}
