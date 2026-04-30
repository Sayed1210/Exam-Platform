namespace Exam.Service;


public interface ICandidateService
{
    Task<List<CandidateResponse>> GetAllCandidates();
    Task<CandidateResponse?> GetCandidateById(int id);

    Task<CandidateResponse?> GetCandidateByExamId(int examId);
    
    Task AddCandidate(CreateCandidateRequest candidate);
    Task DeleteCandidate(int id);
}
