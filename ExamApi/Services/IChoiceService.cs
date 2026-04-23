using ExamApi.DTOs.Requests;
using ExamApi.DTOs.Responses;

namespace ExamApi.Services
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
