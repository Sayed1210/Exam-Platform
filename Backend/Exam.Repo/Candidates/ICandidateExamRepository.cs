using Microsoft.EntityFrameworkCore.Storage;
using Exam.Models;
namespace Exam.Repo;
public interface ICandidateExamRepository
{
    Task<Candidate?> GetCandidateAsync(int id);
    Task AddInvitationAsync(CandidateExam invitation);
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task<bool> ExamExistsAsync(int examId);
    Task<List<int>> GetExamQuestionIdsAsync(int examId);
    Task SaveChangesAsync();
    Task<CandidateExam?> GetAsync(int candidateId, int examId);
    Task SaveAsync(CandidateExam candidateExam);

    Task<CandidateExam?> GetByInvitationTokenAsync(string token);
    Task<int> UpdateExpiredExamsAsync();
}