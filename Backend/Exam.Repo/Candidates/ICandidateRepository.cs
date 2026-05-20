namespace Exam.Repo;
using Exam.Data;
using Exam.Models;


// define what database operations exist
public interface ICandidateRepository
{
    Task<List<Candidate>> GetAllAsync();
    Task<int> CountAsync(string? search, int? status, bool noStatus = false);
    Task<List<Candidate>> GetPagedAsync(int page, int pageSize, string? search, int? status, bool noStatus = false);
    Task<Candidate?> GetByIdAsync(int id);
    Task<Candidate?> GetByEmailAsync(string email);
    Task<Candidate?> GetCandidateByExamIdAsync(int examId);
    Task<Candidate?> GetWithExamsAndAnswersAsync(int candidateId);
    Task<List<Candidate>> GetUnassignedCandidatesAsync();
    Task AddAsync(Candidate candidate);
    Task DeleteAsync(int id);
}