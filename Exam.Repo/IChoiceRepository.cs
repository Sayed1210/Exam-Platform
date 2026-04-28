

using Exam.Models;

namespace Exam.Repo
{
    public interface IChoiceRepository
    {
        Task<IEnumerable<Choice>> GetAllByQuestionIdAsync(int questionId);
        Task<Choice?> GetByIdAsync(int id);
        Task<Choice> CreateAsync(Choice choice);
        Task<Choice> UpdateAsync(Choice choice);
        Task DeleteAsync(Choice choice);
        Task<bool> ExistsAsync(int id);
    }
}