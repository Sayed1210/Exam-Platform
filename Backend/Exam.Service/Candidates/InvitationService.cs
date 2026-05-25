using Exam.Models;
using Exam.Repo;
using Exam.Service.EmailTemplates;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Exam.Service;

public class InvitationService(
    ICandidateExamRepository repository,
    IEmailService emailService,
    IConfiguration configuration,
    ILogger<InvitationService> logger
) : IInvitationService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<InvitationService> _logger = logger;

    public async Task<InvitationStatusResponse> SendInvitationAsync(
        SendInvitationRequest request)
    {
        var nowUtc = DateTime.UtcNow;
        var startUtc = GetUtcDate(request.StartDate);
        var expiryUtc = GetUtcDate(request.ExpiryDate);

        using var transaction = await repository.BeginTransactionAsync();

        try
        {
            await CreateInvitationsAsync(request, nowUtc, startUtc, expiryUtc);

            await SaveAndCommitAsync(transaction);

            return new InvitationStatusResponse(
                true,
                "Invitations sent successfully.",
                nowUtc);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "SendInvitationAsync failed — ExamId={ExamId}",
                request.ExamId);

            await transaction.RollbackAsync();

            return new InvitationStatusResponse(
                false,
                "Failed to complete invitation processing. Internal error occurred.",
                nowUtc);
        }
    }

    private async Task CreateInvitationsAsync(
        SendInvitationRequest request,
        DateTime nowUtc,
        DateTime startUtc,
        DateTime expiryUtc)
    {
        foreach (var candidateId in request.CandidateIds)
        {
            await CreateInvitationForCandidateAsync(
                candidateId,
                request.ExamId,
                nowUtc,
                startUtc,
                expiryUtc);
        }
    }

    private async Task CreateInvitationForCandidateAsync(
        int candidateId,
        int examId,
        DateTime nowUtc,
        DateTime startUtc,
        DateTime expiryUtc)
    {
        try
        {
            var candidate = await GetCandidateAsync(candidateId);

            var token = GenerateInvitationToken();

            var invitation = CreateCandidateExam(
                candidate.Id,
                examId,
                token.Hash,
                nowUtc,
                startUtc,
                expiryUtc);

            await repository.AddInvitationAsync(invitation);

            var start_deadline = FormatCandidateDeadline(startUtc);
            var end_deadline = FormatCandidateDeadline(expiryUtc);

            await SendInvitationEmail(
                candidate.Email,
                token.Raw,
                start_deadline,
                end_deadline);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "CreateInvitationForCandidateAsync failed — CandidateId={CandidateId}, ExamId={ExamId}",
                candidateId,
                examId);
        }
    }

    private async Task<Candidate> GetCandidateAsync(int candidateId)
    {
        return (await repository.GetCandidateAsync(candidateId))!;
    }

    private static CandidateExam CreateCandidateExam(
        int candidateId,
        int examId,
        string tokenHash,
        DateTime nowUtc,
        DateTime startUtc,
        DateTime expiryUtc)
    {
        return new CandidateExam
        {
            CandidateId = candidateId,
            ExamId = examId,
            InvitationToken = tokenHash,
            InvitedAt = nowUtc,
            StartDate = startUtc,
            ExpiryDate = expiryUtc,
            Status = ExamStatus.PENDING
        };
    }

    private static InvitationToken GenerateInvitationToken()
    {
        var rawToken = Guid.NewGuid().ToString();

        var tokenBytes = Encoding.UTF8.GetBytes(rawToken);

        var hashBytes = SHA256.HashData(tokenBytes);

        var tokenHash = Convert.ToHexString(hashBytes);

        return new InvitationToken(rawToken, tokenHash);
    }

    private static DateTime GetUtcDate(DateTime invitationExpiryDate)
    {
        return DateTime.SpecifyKind(
            invitationExpiryDate,
            DateTimeKind.Utc);
    }

    private static string FormatCandidateDeadline(DateTime expiryUtc)
    {
        var candidateZone = GetCandidateTimeZone();

        var candidateLocalTime =
            TimeZoneInfo.ConvertTimeFromUtc(
                expiryUtc,
                candidateZone);

        var timezoneAbbreviation =
            candidateZone.IsDaylightSavingTime(candidateLocalTime)
                ? "EEST"
                : "EET";

        return candidateLocalTime.ToString(
            "MMMM dd, yyyy 'at' hh:mm tt ")
            + timezoneAbbreviation;
    }

    private static TimeZoneInfo GetCandidateTimeZone()
    {
        string candidateTimezoneId =
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? "Egypt Standard Time"
                : "Africa/Cairo";

        return TimeZoneInfo.FindSystemTimeZoneById(candidateTimezoneId);
    }

    private async Task SaveAndCommitAsync(
        IDbContextTransaction transaction)
    {
        try
        {
            await repository.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "SaveAndCommitAsync failed");

            await transaction.RollbackAsync();
        }
    }

    private async Task SendInvitationEmail(
        string email,
        string token,
        string startDeadlineStr,
        string endDeadlineStr)
    {
        try
        {
            var baseUrl = _configuration["Frontend:BaseUrl"];

            var invitationLink =
                $"{baseUrl}/start-exam?token={token}";

            string subject =
                "Action Required: Your Enozom Examination Invitation";

            string body =
                EmailTemplateBuilder.BuildExamInvitation(
                    invitationLink,
                    startDeadlineStr,
                    endDeadlineStr);

            await emailService.SendEmailAsync(
                email,
                subject,
                body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "SendInvitationEmail failed — Email={Email}",
                email);
        }
    }

    private sealed record InvitationToken(
        string Raw,
        string Hash);
}