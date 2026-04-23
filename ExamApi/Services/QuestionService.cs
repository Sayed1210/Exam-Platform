using ExamApi.DTOs.Requests;
using ExamApi.DTOs.Responses;
using ExamApi.Repositories;

namespace ExamApi.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _repo;

        public QuestionService(IQuestionRepository repo) => _repo = repo;

        public async Task<IEnumerable<QuestionResponse>> GetAllAsync()
        {
            var questions = await _repo.GetAllAsync();
            return questions.Select(MapToResponse);
        }

        public async Task<IEnumerable<QuestionResponse>> GetByTopicIdAsync(int topicId)
        {
            var questions = await _repo.GetByTopicIdAsync(topicId);
            return questions.Select(MapToResponse);
        }

        public async Task<QuestionResponse?> GetByIdAsync(int id)
        {
            var question = await _repo.GetByIdAsync(id);
            return question is null ? null : MapToResponse(question);
        }

        public async Task<QuestionResponse> CreateAsync(QuestionRequest request)
        {
            var question = new Question
            {
                TopicId = request.TopicId,
                Text = request.Text,
                ImageUrl = request.ImageUrl
            };

            var created = await _repo.CreateAsync(question);

            // Reload with navigation properties
            var result = await _repo.GetByIdAsync(created.Id);
            return MapToResponse(result!);
        }

        public async Task<QuestionResponse?> UpdateAsync(int id, QuestionRequest request)
        {
            var question = await _repo.GetByIdAsync(id);
            if (question is null) return null;

            question.TopicId = request.TopicId;
            question.Text = request.Text;
            question.ImageUrl = request.ImageUrl;

            await _repo.UpdateAsync(question);

            var result = await _repo.GetByIdAsync(id);
            return MapToResponse(result!);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var question = await _repo.GetByIdAsync(id);
            if (question is null) return false;

            await _repo.DeleteAsync(question);
            return true;
        }

        // ── Mapper ────────────────────────────────────────────────
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
