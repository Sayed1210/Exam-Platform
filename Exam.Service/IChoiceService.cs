using Exam.Models.dtos.requests;
using Exam.Models.dtos.responses;

namespace Exam.Service
{
    public interface IChoiceService
    {
        Task<IEnumerable<ChoiceResponse>> GetAllByQuestionIdAsync(int questionId);
        Task<ChoiceResponse?> GetByIdAsync(int id);
        Task<ChoiceResponse> CreateAsync(ChoiceRequest request);
        Task<ChoiceResponse?> UpdateAsync(int id, ChoiceRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
