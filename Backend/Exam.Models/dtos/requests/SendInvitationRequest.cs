using System.Text.Json.Serialization;

namespace Exam.Models;

public record SendInvitationRequest(
    int ExamId, 
    List<int> CandidateIds,
    [property: JsonPropertyName("startDate")] DateTime StartDate,
    [property: JsonPropertyName("expiryDate")] DateTime ExpiryDate
);
public record InvitationValidationResult(
    bool IsValid, 
    string Message
);