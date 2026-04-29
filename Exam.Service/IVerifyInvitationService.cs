using Exam.Models;
namespace Exam.Services;

public interface IVerifyInvitationService
{
    Task<VerifyInvitationResponse?> VerifyInvitation(string token);
}