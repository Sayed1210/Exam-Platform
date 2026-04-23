using ExamApi.DTOs.Requests;
using ExamApi.DTOs.Responses;
using ExamApi.Repositories;

namespace ExamApi.Services
{
    public class TopicService : ITopicService
    {
        private readonly ITopicRepository _repo;

        public TopicService(ITopicRepository repo)
        {
            _repo = repo;
        }

      
        // CREATE
      
        public async Task<TopicResponse> CreateTopicAsync(TopicRequest dto)
        {
            // 1. Check if title already exists
            if (await _repo.TitleExistsAsync(dto.Title))
                throw new ArgumentException(
                    $"Topic with title '{dto.Title}' already exists");

            // 2. Create entity from DTO
            var topic = new Topic
            {
                Title = dto.Title
            };

            // 3. Save to database
            await _repo.AddAsync(topic);

            // 4. Return response DTO
            return MapToResponseDto(topic);
        }

        // READ - Get All
    
        public async Task<IEnumerable<TopicResponse>> GetAllTopicsAsync()
        {
            var topics = await _repo.GetAllAsync();
            return topics.Select(MapToResponseDto);
        }

       
        // READ - Get By Id
    
        public async Task<TopicResponse?> GetTopicByIdAsync(int id)
        {
            var topic = await _repo.GetByIdAsync(id);

            if (topic == null)
                return null;

            return MapToResponseDto(topic);
        }

        // UPDATE
        
        public async Task<TopicResponse?> UpdateTopicAsync(int id, TopicRequest dto)
        {
            // 1. Find existing topic
            var topic = await _repo.GetByIdAsync(id);

            // 2. If not found, return null
            if (topic == null)
                return null;

            // 3. Check title uniqueness (exclude current topic)
            if (await _repo.TitleExistsAsync(dto.Title, id))
                throw new ArgumentException(
                    $"Topic with title '{dto.Title}' already exists");

            // 4. Update entity properties
            topic.Title = dto.Title;

            // 5. Save changes
            await _repo.UpdateAsync(topic);

            // 6. Return updated DTO
            return MapToResponseDto(topic);
        }

        
        // DELETE
        
        public async Task<bool> DeleteTopicAsync(int id)
        {
            // 1. Find topic
            var topic = await _repo.GetByIdAsync(id);

            // 2. If not found, return false
            if (topic == null)
                return false;

           

            // 3. Delete topic
            await _repo.DeleteAsync(topic);

            // 4. Return success
            return true;
        }

        //
        // HELPER - Map Entity to DTO
        // 
        private TopicResponse MapToResponseDto(Topic topic)
        {
            return new TopicResponse
            {
                Id = topic.Id,
                Title = topic.Title,
                QuestionsCount = topic.Questions?.Count ?? 0
            };
        }
    }
}
