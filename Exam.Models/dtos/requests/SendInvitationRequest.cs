namespace Exam.Models;

public record SendInvitationRequest(
    int ExamId, 
    string Email,
    string FirstName,
    string LastName
);