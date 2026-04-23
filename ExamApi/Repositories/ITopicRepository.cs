namespace ExamApi.Repositories
{
    public interface ITopicRepository
    {
        Task AddAsync(Topic topic);
        Task<IEnumerable<Topic>> GetAllAsync();
        Task<Topic?> GetByIdAsync(int id);
        Task UpdateAsync(Topic topic);
        Task DeleteAsync(Topic topic);
        Task<bool> ExistsAsync(int id);
        Task<bool> TitleExistsAsync(string title, int? excludeId = null);
    }
}
