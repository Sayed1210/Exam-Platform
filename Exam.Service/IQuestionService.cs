using Exam.Models.dtos.requests;
using Exam.Models.dtos.responses;

namespace Exam.Service
{
    public interface IQuestionService
    {
        Task<IEnumerable<QuestionResponse>> GetAllAsync();
        Task<IEnumerable<QuestionResponse>> GetByTopicIdAsync(int topicId);
        Task<QuestionResponse?> GetByIdAsync(int id);
        Task<QuestionResponse> CreateAsync(QuestionRequest request);
        Task<PagedResponse<QuestionResponse>> GetPagedAsync(int page, int pageSize);
        Task<QuestionResponse?> UpdateAsync(int id, UpdateQuestionRequest request);
        Task<QuestionResponse?> UpdateChoiceAsync(int questionId, int choiceId, ChoiceRequest request);
        Task<bool> DeleteAsync(int id);
    }

}
