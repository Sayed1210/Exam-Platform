using Exam.Models;

namespace Exam.Service
{
    public interface ITopicService
    {
        Task<TopicResponse> CreateTopicAsync(TopicRequest dto);
        Task<IEnumerable<TopicResponse>> GetAllTopicsAsync();
        Task<TopicResponse?> GetTopicByIdAsync(int id);
        Task<TopicResponse?> UpdateTopicAsync(int id, TopicRequest dto);
        Task<bool> DeleteTopicAsync(int id);
    }
}
