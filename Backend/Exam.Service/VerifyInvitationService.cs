using Exam.Models;
using Exam.Repo;
using System.Text;
using System.Security.Cryptography;
namespace Exam.Service;

public class VerifyInvitationService :  IVerifyInvitationService
{
    private readonly ICandidateExamRepository _repo;
    public VerifyInvitationService(ICandidateExamRepository repo)
    {
        _repo = repo;
    }

    public async Task<VerifyInvitationResponse?> VerifyInvitation(string token)
    {

        var tokenBytes = Encoding.UTF8.GetBytes(token);
        var hashBytes = SHA256.HashData(tokenBytes);
        var tokenHash = Convert.ToHexString(hashBytes);
        
        var candidateExam = await _repo.GetByInvitationTokenAsync(tokenHash);

        if (candidateExam == null)
            return null;

        // Check if invitation expired
        if (candidateExam.ExpiryDate < DateTime.UtcNow
            || candidateExam.Status == ExamStatus.DONE
            || candidateExam.JoinedAt.HasValue)
        {
                return null;
        }

        return new VerifyInvitationResponse
        {
            CandidateId = candidateExam.CandidateId,
            ExamId = candidateExam.ExamId,
            CandidateName = candidateExam.Candidate!.FirstName + " " + candidateExam.Candidate!.LastName,
            Email = candidateExam.Candidate!.Email
        };
    }
}