namespace ExamApi.Services;


public interface ICandidateService
{
    Task<List<CandidateResponse>> GetAllCandidates();
    Task<CandidateResponse?> GetCandidateById(int id);

    Task<CandidateResponse?> GetCandidateByExamId(int examId);
    
    Task AddCandidate(CreateCandidate candidate);
    Task DeleteCandidate(int id);
}
