using Exam.Models;
namespace Exam.Service;
public interface IInvitationService
{
    Task<InvitationStatusResponse> SendInvitationAsync(SendInvitationRequest request);
}