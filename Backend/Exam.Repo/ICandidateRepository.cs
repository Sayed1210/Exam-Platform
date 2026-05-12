namespace Exam.Repo;
using Exam.Data;
using Exam.Models;


// define what database operations exist
public interface ICandidateRepository
{
    Task<List<Candidate>> GetAllAsync();
    Task<Candidate?> GetByIdAsync(int id);
    // to help check if candidate already exists
    Task<Candidate?> GetByEmailAsync(string email);

    Task<Candidate?> GetCandidateByExamIdAsync(int examId);
    Task<Candidate?> GetWithExamsAndAnswersAsync(int candidateId);

    Task AddAsync(Candidate candidate);
    Task DeleteAsync(int id);

}