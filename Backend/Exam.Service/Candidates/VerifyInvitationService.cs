/////////////////////////////////
using Exam.Models;
using Exam.Repo;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace Exam.Service;

public class VerifyInvitationService : IVerifyInvitationService
{
    private readonly ICandidateExamRepository _repo;
    private readonly ILogger<VerifyInvitationService> _logger;

    public VerifyInvitationService(ICandidateExamRepository repo, ILogger<VerifyInvitationService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<VerifyInvitationResponse?> VerifyInvitation(string token)
    {
        try
        {
            var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            var tokenHash = Convert.ToHexString(hashBytes);

            var candidateExam = await _repo.GetByInvitationTokenAsync(tokenHash);

            if (candidateExam is null)
                return null;

            if (candidateExam.ExpiryDate < DateTime.UtcNow
                || candidateExam.Status == ExamStatus.DONE
                || candidateExam.JoinedAt.HasValue)
                return null;

            return new VerifyInvitationResponse
            {
                CandidateId = candidateExam.CandidateId,
                ExamId = candidateExam.ExamId,
                CandidateName = candidateExam.Candidate!.FirstName + " " + candidateExam.Candidate!.LastName,
                Email = candidateExam.Candidate!.Email
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "VerifyInvitation failed — Token={Token}", token);
            return null;
        }
    }
}