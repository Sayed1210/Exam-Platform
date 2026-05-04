using Exam.Models;
using Exam.Repo;

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
        var candidateExam = await _repo.GetByInvitationTokenAsync(token);

        if (candidateExam == null)
            return null;

        // Check if invitation expired
        if (candidateExam.ExpiryDate < DateTime.UtcNow)
            return null;

        return new VerifyInvitationResponse
        {
            CandidateId = candidateExam.CandidateId,
            ExamId = candidateExam.ExamId,
            CandidateName = candidateExam.Candidate!.FirstName + " " + candidateExam.Candidate!.LastName,
            Email = candidateExam.Candidate!.Email
        };
    }
}