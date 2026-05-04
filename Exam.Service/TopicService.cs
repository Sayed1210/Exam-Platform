using Exam.Models.dtos.requests;
using Exam.Models.dtos.responses;
using Exam.Models;
using Exam.Repo;

namespace Exam.Service
{
    public class TopicService : ITopicService
    {
        private readonly ITopicRepository _repo;

        public TopicService(ITopicRepository repo) => _repo = repo;

        // ── CREATE 
        public async Task<TopicResponse> CreateTopicAsync(TopicRequest dto)
        {
           

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
            var topic = await _repo.GetByIdAsync(id);
            return topic is null ? null : MapToResponseDto(topic);
        }

        // ── UPDATE ────────────────────────────────────────────────────
        public async Task<TopicResponse?> UpdateTopicAsync(int id, TopicRequest dto)
        {
   

            var topic = await _repo.GetByIdAsync(id);
            if (topic is null) return null;

            topic.Title = dto.Title.Trim();

            await _repo.UpdateAsync(topic);
            return MapToResponseDto(topic);
        }

        // ── DELETE ────────────────────────────────────────────────────
        public async Task<bool> DeleteTopicAsync(int id)
        {
            var topic = await _repo.GetByIdAsync(id);
            if (topic is null) return false;
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
