using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Exam.Data;
using Exam.Models;
namespace Exam.Repo;
public class CandidateExamRepository(ApiContext context) : ICandidateExamRepository
{
    public async Task<Candidate?> GetCandidateByEmailAsync(string email)
    {
        return await context.Candidates.AsNoTracking().FirstOrDefaultAsync(c => c.Email == email);
    }

    public async Task AddInvitationAsync(CandidateExam invitation)
    {
        await context.CandidateExams.AddAsync(invitation);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await context.Database.BeginTransactionAsync();
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}
