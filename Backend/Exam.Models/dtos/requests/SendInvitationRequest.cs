using System.Text.Json.Serialization;

namespace Exam.Models;

public record SendInvitationRequest(
    int ExamId, 
    List<int> CandidateIds,
    [property: JsonPropertyName("expiryDate")] DateTime InvitationExpiryDate
);
public record InvitationValidationResult(
    bool IsValid, 
    string Message
);