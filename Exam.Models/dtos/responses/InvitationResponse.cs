namespace Exam.Models.Dtos.Responses;
public record InvitationStatusResponse(
    bool IsSuccess,
    string Message,
    DateTime SentAt
);