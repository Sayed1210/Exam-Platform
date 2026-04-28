namespace Exam.Models.Dtos.Requests;

public record SendInvitationRequest(
    int ExamId, 
    string Email,
    string FirstName,
    string LastName
);