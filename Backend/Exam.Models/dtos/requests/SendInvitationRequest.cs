namespace Exam.Models;

public record SendInvitationRequest(
    int ExamId, 
    List<int> CandidateIds,
    DateTime InvitationExpiryDate
);
public record InvitationValidationResult(
    bool IsValid, 
    string Message
);