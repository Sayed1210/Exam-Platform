using Microsoft.EntityFrameworkCore.Storage;
using Exam.Models;
namespace Exam.Repo;
public interface ICandidateExamRepository
{
    Task<Candidate?> GetCandidateByEmailAsync(string email);
    Task AddInvitationAsync(CandidateExam invitation);
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task SaveChangesAsync();
    Task<CandidateExam?> GetAsync(int candidateId, int examId);
    Task SaveAsync(CandidateExam candidateExam);

    Task<CandidateExam?> GetByInvitationTokenAsync(string token);

}