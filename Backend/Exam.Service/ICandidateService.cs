namespace Exam.Service;
using Exam.Models;


public interface ICandidateService
{
    Task<List<CandidateResponse>> GetAllCandidates();
    Task<CandidateResponse?> GetCandidateById(int id);

    Task<CandidateResponse?> GetCandidateByExamId(int examId);
    
    Task<bool> AddCandidate(CreateCandidateRequest candidate);
    Task<CandidateDetailResponse?> GetDetailAsync(int candidateId);
    Task<bool> DeleteCandidate(int id);
}
