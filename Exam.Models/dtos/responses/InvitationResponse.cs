namespace Exam.Models;
public record InvitationStatusResponse(
    bool IsSuccess,
    string Message,
    DateTime SentAt
);