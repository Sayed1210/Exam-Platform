namespace Exam.Service;
using Exam.Models;


public interface ICandidateService
{
    
     Task<PagedResponse<CandidateResponse>> GetAllCandidates(int page, int pageSize, string? search, int? status, bool noStatus = false);
     Task<CandidateResponse?> GetCandidateById(int id);

    Task<CandidateResponse?> GetCandidateByExamId(int examId);
    
    Task<bool> AddCandidate(CreateCandidateRequest candidate);
    Task<CandidateDetailResponse?> GetDetailAsync(int candidateId);
    Task<List<CandidateResponse>> GetUnassignedCandidates();
    Task<bool> DeleteCandidate(int id);
}
