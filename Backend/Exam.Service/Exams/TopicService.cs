using Exam.Models;
using Exam.Repo;
using Microsoft.Extensions.Logging;

namespace Exam.Service
{
    public class TopicService : ITopicService
    {
        private readonly ITopicRepository _repo;
        private readonly ILogger<TopicService> _logger;

        public TopicService(ITopicRepository repo, ILogger<TopicService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<TopicResponse> CreateTopicAsync(TopicRequest dto)
        {
            try
            {
                var topic = new Topic { Title = dto.Title.Trim() };
                await _repo.AddAsync(topic);
                return MapToResponseDto(topic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateTopicAsync failed — Title={Title}", dto.Title);
                return new TopicResponse();
            }
        }

        public async Task<IEnumerable<TopicResponse>> GetAllTopicsAsync()
        {
            var topics = await _repo.GetAllAsync();
            return topics.Select(MapToResponseDto);
        }

        public async Task<TopicResponse?> GetTopicByIdAsync(int id)
        {
            var topic = await _repo.GetByIdAsync(id);
            return topic is null ? null : MapToResponseDto(topic);
        }

        public async Task<TopicResponse?> UpdateTopicAsync(int id, TopicRequest dto)
        {
            try
            {
                var topic = await _repo.GetByIdAsync(id);
                if (topic is null) return null;

                topic.Title = dto.Title.Trim();
                await _repo.UpdateAsync(topic);
                return MapToResponseDto(topic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateTopicAsync failed — TopicId={TopicId}", id);
                return null;
            }
        }

        public async Task<bool> DeleteTopicAsync(int id)
        {
            try
            {
                var topic = await _repo.GetByIdAsync(id);
                if (topic is null) return false;

                await _repo.DeleteAsync(topic);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteTopicAsync failed — TopicId={TopicId}", id);
                return false;
            }
        }

        private static TopicResponse MapToResponseDto(Topic topic) => new()
        {
            Id = topic.Id,
            Title = topic.Title,
            QuestionsCount = topic.Questions?.Count ?? 0
        };
    }
}