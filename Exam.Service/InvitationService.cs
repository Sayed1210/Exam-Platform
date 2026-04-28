using Exam.Models;
using Exam.Models.Dtos.Requests;
using Exam.Models.Dtos.Responses;
using Exam.Repo;
namespace Exam.Service;
public class InvitationService(ICandidateExamRepository repository, IEmailService emailService) : IInvitationService
{
    public async Task<InvitationStatusResponse> SendInvitationAsync(SendInvitationRequest request)
    {
        var nowUtc = DateTime.UtcNow;
        
        // 1. Check if candidate exists using the Repo
        var candidate = await repository.GetCandidateByEmailAsync(request.Email);
        if (candidate == null)
        {
            return new InvitationStatusResponse(false, "Candidate with this email not found", nowUtc);
        }

        using var transaction = await repository.BeginTransactionAsync();
        try
        {
            var invitation = new CandidateExam
            {
                CandidateId = candidate.Id,
                ExamId = request.ExamId,
                InvitationToken = Guid.NewGuid().ToString(),
                InvitedAt = nowUtc,
                ExpiryDate = nowUtc.AddDays(3),
                Status = ExamStatus.PENDING
            };

            await repository.AddInvitationAsync(invitation);
            await repository.SaveChangesAsync();

            
            await emailService.SendInvitationEmail(request.Email, invitation.InvitationToken);

            
            await transaction.CommitAsync();
            
            return new InvitationStatusResponse(true, "Invitation sent successfully.", nowUtc);
        }
        catch (Exception)
        {

            await transaction.RollbackAsync();
            return new InvitationStatusResponse(false, "Failed to send invitation. Changes rolled back.", nowUtc);
        }
    }
}