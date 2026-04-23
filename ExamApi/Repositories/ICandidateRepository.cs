namespace ExamApi.Repositories;
// define what database operations exist
public interface ICandidateRepository
{
    Task<List<Candidate>> GetAllAsync();
    Task<Candidate?> GetByIdAsync(int id);

    Task<Candidate?> GetByEmailAsync(string email);

    Task AddAsync(Candidate candidate);
    Task DeleteAsync(int id);

}