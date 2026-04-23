// CandidateExam table -> update 'score' & 'status' values in db
namespace ExamApi.Repositories;

public interface ICandidateExamRepository
{
    Task<CandidateExam?> GetAsync(int candidateId, int examId);
    Task SaveAsync(CandidateExam candidateExam);
}
