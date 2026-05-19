using Exam.Models;
using Exam.Repo;
namespace Exam.Service;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Security.Cryptography;

public class InvitationService(ICandidateExamRepository repository, IEmailService emailService) : IInvitationService
{
    public async Task<InvitationStatusResponse> SendInvitationAsync(SendInvitationRequest request)
    {
        var nowUtc = DateTime.UtcNow;
        var expiryUtc = DateTime.SpecifyKind(request.InvitationExpiryDate, DateTimeKind.Utc);
        using var transaction = await repository.BeginTransactionAsync();
        
        try
        {
            foreach (var candidateId in request.CandidateIds)
            {
                // Safe to use '!' because the Validator service already verified existence
                var candidate = await repository.GetCandidateAsync(candidateId);
                var rawToken   = Guid.NewGuid().ToString();
                var tokenBytes = Encoding.UTF8.GetBytes(rawToken);
                var hashBytes  = SHA256.HashData(tokenBytes);
                var tokenHash  = Convert.ToHexString(hashBytes);
                string candidateTimezoneId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                    ? "Egypt Standard Time"
                    : "Africa/Cairo";
                TimeZoneInfo candidateZone = TimeZoneInfo.FindSystemTimeZoneById(candidateTimezoneId);
                DateTime candidateLocalTime = TimeZoneInfo.ConvertTimeFromUtc(expiryUtc, candidateZone);
                var invitation = new CandidateExam
                {
                    CandidateId = candidate!.Id,
                    ExamId = request.ExamId,
                    InvitationToken = tokenHash,
                    InvitedAt = nowUtc,
                    ExpiryDate = expiryUtc,
                    Status = ExamStatus.PENDING
                };

                await repository.AddInvitationAsync(invitation);
                
                // Send the beautifully styled email template with the dynamic deadline
                string formattedDeadline = candidateLocalTime.ToString("MMMM dd, yyyy 'at' hh:mm tt ") + (candidateZone.IsDaylightSavingTime(candidateLocalTime) ? "EEST" : "EET");
                await SendInvitationEmail(candidate.Email, rawToken, formattedDeadline);
            }

            await repository.SaveChangesAsync();
            await transaction.CommitAsync();
            return new InvitationStatusResponse(true, "Invitations sent successfully.", nowUtc);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return new InvitationStatusResponse(false, "Failed to complete invitation processing. Internal error occurred.", nowUtc);
        }
    }

    private async Task SendInvitationEmail(string email, string token, string deadlineStr)
    {
        var invitationLink = $"http://localhost:5173/join-exam?token={token}";
        string subject = "Action Required: Your Enozom Examination Invitation";
        string body = $@"
            <div style='background-color: #f4f4f7; padding: 30px; font-family: sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 8px; border: 1px solid #e1e1e1;'>
                    <div style='background-color: #2c3e50; padding: 20px; text-align: center; border-radius: 8px 8px 0 0;'>
                        <h1 style='color: #ffffff; margin: 0; font-size: 22px;'>Examination Invitation</h1>
                    </div>
                    <div style='padding: 30px;'>
                        <p style='font-size: 16px; color: #333;'>Hello,</p>
                        <p style='font-size: 16px; color: #555; line-height: 1.5;'>
                            You have been invited to complete an assessment on the <b>Enozom</b> platform. 
                        </p>
                        <div style='background-color: #fff5f5; border-left: 4px solid #c0392b; padding: 15px; margin: 20px 0;'>
                            <p style='margin: 0; color: #c0392b; font-weight: bold; font-size: 14px;'>
                                DEADLINE: {deadlineStr}
                            </p>
                            <p style='margin: 5px 0 0 0; color: #7f8c8d; font-size: 13px;'>
                                Please ensure you start the exam before this date, as your link will expire.
                            </p>
                        </div>
                        <div style='text-align: center; margin: 30px 0;'>
                            <a href='{invitationLink}' style='background-color: #2c3e50; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;'>
                                Begin Examination
                            </a>
                        </div>
                    </div>
                    <div style='padding: 20px; background-color: #f9f9f9; text-align: center; font-size: 12px; color: #999;'>
                        This is an automated message. Please do not reply.
                    </div>
                </div>
            </div>";

        await emailService.SendEmailAsync(email, subject, body);
    }
}