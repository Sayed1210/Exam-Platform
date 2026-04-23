using ExamApi.DTOs.Requests;
using ExamApi.DTOs.Responses;

namespace ExamApi.Services
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
