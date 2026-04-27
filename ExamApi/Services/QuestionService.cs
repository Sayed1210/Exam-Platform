using ExamApi.DTOs.Requests;
using ExamApi.DTOs.Responses;
using ExamApi.Repositories;

namespace ExamApi.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _repo;

        public QuestionService(IQuestionRepository repo) => _repo = repo;

        // ── GET ALL ───────────────────────────────────────────────────
        public async Task<IEnumerable<QuestionResponse>> GetAllAsync()
        {
            var questions = await _repo.GetAllAsync();
            return questions.Select(MapToResponse);
        }

        // ── GET BY TOPIC ──────────────────────────────────────────────
        public async Task<IEnumerable<QuestionResponse>> GetByTopicIdAsync(int topicId)
        {
            if (topicId <= 0)
                throw new ArgumentException("TopicId must be a positive number.", nameof(topicId));

            var questions = await _repo.GetByTopicIdAsync(topicId);
            return questions.Select(MapToResponse);
        }

        // ── GET BY ID ─────────────────────────────────────────────────
        public async Task<QuestionResponse?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be a positive number.", nameof(id));

            var question = await _repo.GetByIdAsync(id);
            return question is null ? null : MapToResponse(question);
        }

        // ── CREATE ────────────────────────────────────────────────────
        public async Task<QuestionResponse> CreateAsync(QuestionRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (request.TopicId <= 0)
                throw new ArgumentException("TopicId must be a positive number.", nameof(request.TopicId));

            if (string.IsNullOrWhiteSpace(request.Text))
                throw new ArgumentException("Question text cannot be empty.", nameof(request.Text));

            if (request.Text.Length > 1000)
                throw new ArgumentException("Question text cannot exceed 1000 characters.", nameof(request.Text));

            var question = new Question
            {
                TopicId = request.TopicId,
                Text = request.Text.Trim(),
                ImageUrl = request.ImageUrl
            };

            var created = await _repo.CreateAsync(question);
            var result = await _repo.GetByIdAsync(created.Id);
            return MapToResponse(result!);
        }

        // ── UPDATE ────────────────────────────────────────────────────
        public async Task<QuestionResponse?> UpdateAsync(int id, QuestionRequest request)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be a positive number.", nameof(id));

            ArgumentNullException.ThrowIfNull(request);

            if (request.TopicId <= 0)
                throw new ArgumentException("TopicId must be a positive number.", nameof(request.TopicId));

            if (string.IsNullOrWhiteSpace(request.Text))
                throw new ArgumentException("Question text cannot be empty.", nameof(request.Text));

            if (request.Text.Length > 1000)
                throw new ArgumentException("Question text cannot exceed 1000 characters.", nameof(request.Text));

            var question = await _repo.GetByIdAsync(id);
            if (question is null) return null;

            question.TopicId = request.TopicId;
            question.Text = request.Text.Trim();
            question.ImageUrl = request.ImageUrl;

            await _repo.UpdateAsync(question);

            var result = await _repo.GetByIdAsync(id);
            return MapToResponse(result!);
        }

        // ── DELETE ────────────────────────────────────────────────────
        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be a positive number.", nameof(id));

            var question = await _repo.GetByIdAsync(id);
            if (question is null) return false;

            await _repo.DeleteAsync(question);
            return true;
        }

        // ── Mapper ────────────────────────────────────────────────────
        private static QuestionResponse MapToResponse(Question q) => new()
        {
            Id = q.Id,
            TopicId = q.TopicId,
            TopicTitle = q.Topic?.Title ?? string.Empty,
            Text = q.Text,
            ImageUrl = q.ImageUrl,
            Choices = q.Choices.Select(c => new ChoiceInQuestion
            {
                Id = c.Id,
                Text = c.Text,
                IsCorrect = c.IsCorrect,
                ImageUrl = c.ImageUrl
            }).ToList()
        };
    }
}