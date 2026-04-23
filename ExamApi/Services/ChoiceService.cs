using ExamApi.DTOs.Requests;
using ExamApi.DTOs.Responses;
using ExamApi.Repositories;

namespace ExamApi.Services
{
    public class ChoiceService : IChoiceService
    {
        private readonly IChoiceRepository _repo;

        public ChoiceService(IChoiceRepository repo) => _repo = repo;

        public async Task<IEnumerable<ChoiceResponse>> GetAllByQuestionIdAsync(int questionId)
        {
            var choices = await _repo.GetAllByQuestionIdAsync(questionId);
            return choices.Select(MapToResponse);
        }

        public async Task<ChoiceResponse?> GetByIdAsync(int id)
        {
            var choice = await _repo.GetByIdAsync(id);
            return choice is null ? null : MapToResponse(choice);
        }

        public async Task<ChoiceResponse> CreateAsync(ChoiceRequest request)
        {
            var choice = new Choice
            {
                QuestionId = request.QuestionId,
                Text = request.Text,
                IsCorrect = request.IsCorrect,
                ImageUrl = request.ImageUrl
            };

            var created = await _repo.CreateAsync(choice);
            return MapToResponse(created);
        }

        public async Task<ChoiceResponse?> UpdateAsync(int id, ChoiceRequest request)
        {
            var choice = await _repo.GetByIdAsync(id);
            if (choice is null) return null;

            choice.Text = request.Text;
            choice.IsCorrect = request.IsCorrect;
            choice.ImageUrl = request.ImageUrl;
            // QuestionId is intentionally not updated —
            // moving a choice to a different question should not be allowed.

            var updated = await _repo.UpdateAsync(choice);
            return MapToResponse(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var choice = await _repo.GetByIdAsync(id);
            if (choice is null) return false;

            await _repo.DeleteAsync(choice);
            return true;
        }

        // ── Mapper
        private static ChoiceResponse MapToResponse(Choice c) => new()
        {
            Id = c.Id,
            QuestionId = c.QuestionId,
            Text = c.Text,
            IsCorrect = c.IsCorrect,
            ImageUrl = c.ImageUrl
        };
    }
}
