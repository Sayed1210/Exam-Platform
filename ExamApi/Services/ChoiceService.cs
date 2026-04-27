using ExamApi.DTOs.Requests;
using ExamApi.DTOs.Responses;
using ExamApi.Repositories;

namespace ExamApi.Services
{
    public class ChoiceService : IChoiceService
    {
        private readonly IChoiceRepository _repo;
        private readonly IQuestionRepository _questionRepo;

        public ChoiceService(
            IChoiceRepository repo,
            IQuestionRepository questionRepo)
        {
            _repo = repo;
            _questionRepo = questionRepo;
        }

        // ── GET all by question ───────────────────────────────────────
        public async Task<IEnumerable<ChoiceResponse>> GetAllByQuestionIdAsync(int questionId)
        {
            // Validate: question must exist
            var questionExists = await _questionRepo.ExistsAsync(questionId);
            if (!questionExists)
                throw new KeyNotFoundException($"Question {questionId} not found.");

            var choices = await _repo.GetAllByQuestionIdAsync(questionId);
            return choices.Select(MapToResponse);
        }

        // ── GET by id ─────────────────────────────────────────────────
        public async Task<ChoiceResponse?> GetByIdAsync(int id)
        {
            // Validate: id must be positive
            if (id <= 0)
                throw new ArgumentException("Id must be a positive number.", nameof(id));

            var choice = await _repo.GetByIdAsync(id);
            return choice is null ? null : MapToResponse(choice);
        }

        // ── CREATE ────────────────────────────────────────────────────
        public async Task<ChoiceResponse> CreateAsync(ChoiceRequest request)
        {
            // Validate: request must not be null
            ArgumentNullException.ThrowIfNull(request);

            // Validate: text must not be empty
            if (string.IsNullOrWhiteSpace(request.Text))
                throw new ArgumentException("Choice text cannot be empty.", nameof(request.Text));

            // Validate: text max length
            if (request.Text.Length > 500)
                throw new ArgumentException("Choice text cannot exceed 500 characters.", nameof(request.Text));

            // Validate: question must exist
            var questionExists = await _questionRepo.ExistsAsync(request.QuestionId);
            if (!questionExists)
                throw new KeyNotFoundException($"Question {request.QuestionId} not found.");

            // Validate: a question cannot have more than 6 choices
            var existingChoices = await _repo.GetAllByQuestionIdAsync(request.QuestionId);
            if (existingChoices.Count() >= 6)
                throw new InvalidOperationException("A question cannot have more than 6 choices.");

            // Validate: no duplicate text for the same question (case-insensitive)
            var isDuplicate = existingChoices
                .Any(c => c.Text.Equals(request.Text, StringComparison.OrdinalIgnoreCase));
            if (isDuplicate)
                throw new InvalidOperationException(
                    $"A choice with the text '{request.Text}' already exists for this question.");

            // Validate: imageUrl format if provided
            if (!string.IsNullOrEmpty(request.ImageUrl) && !Uri.IsWellFormedUriString(request.ImageUrl, UriKind.Absolute))
                throw new ArgumentException("ImageUrl must be a valid URL.", nameof(request.ImageUrl));

            var choice = new Choice
            {
                QuestionId = request.QuestionId,
                Text = request.Text.Trim(),
                IsCorrect = request.IsCorrect,
                ImageUrl = request.ImageUrl
            };

            var created = await _repo.CreateAsync(choice);
            return MapToResponse(created);
        }

        // ── UPDATE ────────────────────────────────────────────────────
        public async Task<ChoiceResponse?> UpdateAsync(int id, ChoiceRequest request)
        {
            // Validate: id must be positive
            if (id <= 0)
                throw new ArgumentException("Id must be a positive number.", nameof(id));

            // Validate: request must not be null
            ArgumentNullException.ThrowIfNull(request);

            var choice = await _repo.GetByIdAsync(id);
            if (choice is null) return null;

            // Validate: text must not be empty
            if (string.IsNullOrWhiteSpace(request.Text))
                throw new ArgumentException("Choice text cannot be empty.", nameof(request.Text));

            // Validate: text max length
            if (request.Text.Length > 500)
                throw new ArgumentException("Choice text cannot exceed 500 characters.", nameof(request.Text));

            // Validate: no duplicate text for the same question (excluding current choice)
            var sibling = await _repo.GetAllByQuestionIdAsync(choice.QuestionId);
            var isDuplicate = sibling
                .Any(c => c.Id != id &&
                          c.Text.Equals(request.Text, StringComparison.OrdinalIgnoreCase));
            if (isDuplicate)
                throw new InvalidOperationException(
                    $"Another choice with the text '{request.Text}' already exists for this question.");

            // Validate: imageUrl format if provided
            if (!string.IsNullOrEmpty(request.ImageUrl) && !Uri.IsWellFormedUriString(request.ImageUrl, UriKind.Absolute))
                throw new ArgumentException("ImageUrl must be a valid URL.", nameof(request.ImageUrl));

            // QuestionId is intentionally not updated —
            // moving a choice to a different question should not be allowed.
            choice.Text = request.Text.Trim();
            choice.IsCorrect = request.IsCorrect;
            choice.ImageUrl = request.ImageUrl;

            var updated = await _repo.UpdateAsync(choice);
            return MapToResponse(updated);
        }

        // ── DELETE ────────────────────────────────────────────────────
        public async Task<bool> DeleteAsync(int id)
        {
            // Validate: id must be positive
            if (id <= 0)
                throw new ArgumentException("Id must be a positive number.", nameof(id));

            var choice = await _repo.GetByIdAsync(id);
            if (choice is null) return false;

            await _repo.DeleteAsync(choice);
            return true;
        }

        // ── Mapper ────────────────────────────────────────────────────
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