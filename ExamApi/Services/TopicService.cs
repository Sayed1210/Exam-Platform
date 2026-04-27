using ExamApi.DTOs.Requests;
using ExamApi.DTOs.Responses;
using ExamApi.Repositories;

namespace ExamApi.Services
{
    public class TopicService : ITopicService
    {
        private readonly ITopicRepository _repo;

        public TopicService(ITopicRepository repo) => _repo = repo;

        // ── CREATE ────────────────────────────────────────────────────
        public async Task<TopicResponse> CreateTopicAsync(TopicRequest dto)
        {
            // Validate: request must not be null
            ArgumentNullException.ThrowIfNull(dto);

            // Validate: title must not be empty
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("Topic title cannot be empty.", nameof(dto.Title));

            // Validate: title max length
            if (dto.Title.Length > 100)
                throw new ArgumentException("Topic title cannot exceed 100 characters.", nameof(dto.Title));

            // Validate: no duplicate title (case-insensitive)
            if (await _repo.TitleExistsAsync(dto.Title))
                throw new InvalidOperationException(
                    $"Topic with title '{dto.Title}' already exists.");

            var topic = new Topic
            {
                Title = dto.Title.Trim()
            };

            await _repo.AddAsync(topic);
            return MapToResponseDto(topic);
        }

        // ── GET ALL ───────────────────────────────────────────────────
        public async Task<IEnumerable<TopicResponse>> GetAllTopicsAsync()
        {
            var topics = await _repo.GetAllAsync();
            return topics.Select(MapToResponseDto);
        }

        // ── GET BY ID ─────────────────────────────────────────────────
        public async Task<TopicResponse?> GetTopicByIdAsync(int id)
        {
            // Validate: id must be positive
            if (id <= 0)
                throw new ArgumentException("Id must be a positive number.", nameof(id));

            var topic = await _repo.GetByIdAsync(id);
            return topic is null ? null : MapToResponseDto(topic);
        }

        // ── UPDATE ────────────────────────────────────────────────────
        public async Task<TopicResponse?> UpdateTopicAsync(int id, TopicRequest dto)
        {
            // Validate: id must be positive
            if (id <= 0)
                throw new ArgumentException("Id must be a positive number.", nameof(id));

            // Validate: request must not be null
            ArgumentNullException.ThrowIfNull(dto);

            // Validate: title must not be empty
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("Topic title cannot be empty.", nameof(dto.Title));

            // Validate: title max length
            if (dto.Title.Length > 100)
                throw new ArgumentException("Topic title cannot exceed 100 characters.", nameof(dto.Title));

            var topic = await _repo.GetByIdAsync(id);
            if (topic is null) return null;

            // Validate: no duplicate title (exclude current topic)
            if (await _repo.TitleExistsAsync(dto.Title, id))
                throw new InvalidOperationException(
                    $"Topic with title '{dto.Title}' already exists.");

            topic.Title = dto.Title.Trim();

            await _repo.UpdateAsync(topic);
            return MapToResponseDto(topic);
        }

        // ── DELETE ────────────────────────────────────────────────────
        public async Task<bool> DeleteTopicAsync(int id)
        {
            // Validate: id must be positive
            if (id <= 0)
                throw new ArgumentException("Id must be a positive number.", nameof(id));

            var topic = await _repo.GetByIdAsync(id);
            if (topic is null) return false;

            // Validate: cannot delete a topic that has questions
            if (topic.Questions != null && topic.Questions.Count > 0)
                throw new InvalidOperationException(
                    $"Cannot delete topic '{topic.Title}' because it has {topic.Questions.Count} question(s) linked to it. Remove the questions first.");

            await _repo.DeleteAsync(topic);
            return true;
        }

        // ── HELPER — Map Entity to DTO ────────────────────────────────
        private static TopicResponse MapToResponseDto(Topic topic) => new()
        {
            Id = topic.Id,
            Title = topic.Title,
            QuestionsCount = topic.Questions?.Count ?? 0
        };
    }
}