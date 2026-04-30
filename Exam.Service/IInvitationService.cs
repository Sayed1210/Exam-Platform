using Exam.Models.Dtos.Requests;
using Exam.Models.Dtos.Responses;
namespace Exam.Service;
public interface IInvitationService
{
    Task<InvitationStatusResponse> SendInvitationAsync(SendInvitationRequest request);
}