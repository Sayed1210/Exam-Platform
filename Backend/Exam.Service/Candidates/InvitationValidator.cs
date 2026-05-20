using Exam.Models;
using Exam.Repo;

namespace Exam.Service;
public class InvitationValidator(ICandidateExamRepository repository) : IInvitationValidator
{
    public async Task<InvitationValidationResult> ValidateAsync(SendInvitationRequest request)
    {
        var expiryUtc = DateTime.SpecifyKind(request.InvitationExpiryDate, DateTimeKind.Utc);

        // 1. Validate Expiry Date isn't in the past
        if (expiryUtc <= DateTime.UtcNow)
        {
            return new InvitationValidationResult(false, "The expiration date must be a future date.");
        }
        // 2. Validate Exam Existence
        var examExists = await repository.ExamExistsAsync(request.ExamId);
        if (!examExists)
        {
            return new InvitationValidationResult(false, $"Exam with ID {request.ExamId} does not exist.");
        }

        // 2. Validate every Candidate ID in the array
        foreach (var id in request.CandidateIds)
        {
            var candidate = await repository.GetCandidateAsync(id);
            if (candidate == null)
            {
                return new InvitationValidationResult(false, $"Candidate with ID {id} not found.");
            }

            // Check if this specific candidate is already invited
            var existingInvitation = await repository.GetAsync(id, request.ExamId);
            if (existingInvitation != null)
            {
                return new InvitationValidationResult(false, $"Candidate {candidate.Email} has already been invited to this exam.");
            }
        }

        return new InvitationValidationResult(true, "Validation passed.");
    }
}