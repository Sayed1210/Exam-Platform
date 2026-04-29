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
        
        var candidate = await repository.GetCandidateByEmailAsync(request.Email);
        if (candidate == null)
        {
            return new InvitationStatusResponse(false, "Candidate with this email not found", nowUtc);
        }

        using var transaction = await repository.BeginTransactionAsync();
        try
        {
            var invitationToken=Guid.NewGuid().ToString();
            var invitation = new CandidateExam
            {
                CandidateId = candidate.Id,
                ExamId = request.ExamId,
                InvitationToken = invitationToken,
                InvitedAt = nowUtc,
                ExpiryDate = nowUtc.AddDays(3),
                Status = ExamStatus.PENDING
            };

            await repository.AddInvitationAsync(invitation);
            await repository.SaveChangesAsync();

            var invitationLink=$"http://localhost:5173/join-exam?token={invitationToken}";
            string subject="Invitation to Enozom Examination";
            string body = $@"
                <div style='font-family: sans-serif; line-height: 1.6;'>
                    <h1>Hello!</h1>
                    <p>You have been invited to take an exam on the <b>Enozom</b> platform.</p>
                    <p>Please click the button below to begin your session:</p>
                    <p style='margin: 20px 0;'>
                        <a href='{invitationLink}' 
                           style='background: #2c3e50; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px;'>
                           Start Exam
                        </a>
                    </p>
                    <p><b>Note:</b> This link will expire in 3 days.</p>
                </div>";
            await emailService.SendEmailAsync(request.Email, subject,body);

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