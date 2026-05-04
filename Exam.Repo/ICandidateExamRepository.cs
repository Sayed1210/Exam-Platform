// CandidateExam table -> update 'score' & 'status' values in db
namespace Exam.Repositories;
using Exam.Models;

public interface ICandidateExamRepository
{
    Task<CandidateExam?> GetAsync(int candidateId, int examId);
    Task SaveAsync(CandidateExam candidateExam);

    Task<CandidateExam?> GetByInvitationTokenAsync(string token);
}
