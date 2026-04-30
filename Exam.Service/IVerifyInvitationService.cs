using Exam.Models;
namespace Exam.Service;

public interface IVerifyInvitationService
{
    Task<VerifyInvitationResponse?> VerifyInvitation(string token);
}