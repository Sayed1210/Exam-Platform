using Exam.Models;
using Exam.Repo;

namespace Exam.Service;

public interface IInvitationValidator
{
    Task<InvitationValidationResult> ValidateAsync(SendInvitationRequest request);
}

