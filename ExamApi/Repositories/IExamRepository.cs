namespace ExamApi.Repositories
{
    public interface IExamRepository
    {
        // Create
        Task AddAsync(Exam exam);

        // Read
        Task<IEnumerable<Exam>> GetAllAsync();
        Task<Exam?> GetByIdAsync(int id);

        // Update
        Task UpdateAsync(Exam exam);

        // Delete
        Task DeleteAsync(Exam exam);
    
        // Helper
        Task<bool> ExistsAsync(int id);
    }
  
    
}

