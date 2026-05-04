
using Exam.Models;

namespace Exam.Repo
{
    public interface IQuestionRepository
    {
        Task<IEnumerable<Question>> GetAllAsync();
        Task<IEnumerable<Question>> GetByTopicIdAsync(int topicId);
        Task<Question?> GetByIdAsync(int id);
        Task<Question> CreateAsync(Question question);
        Task<Question> UpdateAsync(Question question, bool updateChoices = false);
        Task<Question> UpdateChoiceAsync(Choice choice);
        Task DeleteAsync(Question question);
        Task<bool> ExistsAsync(int id);
        Task<List<int>> GetExistingIdsAsync(List<int> ids);
        Task<(IEnumerable<Question> Items, int TotalCount)> GetPagedAsync(int page, int pageSize);
    }
}