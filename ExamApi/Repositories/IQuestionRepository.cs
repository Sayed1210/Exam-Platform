namespace ExamApi.Repositories
{
    public interface IQuestionRepository
    {
        Task<IEnumerable<Question>> GetAllAsync();
        Task<IEnumerable<Question>> GetByTopicIdAsync(int topicId);
        Task<Question?> GetByIdAsync(int id);
        Task<Question> CreateAsync(Question question);
        Task<Question> UpdateAsync(Question question);
        Task DeleteAsync(Question question);
        Task<bool> ExistsAsync(int id);
    }

}
