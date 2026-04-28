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
        Task<QuestionResponse?> UpdateAsync(int id, QuestionRequest request);
        Task<bool> DeleteAsync(int id);
    }

}
