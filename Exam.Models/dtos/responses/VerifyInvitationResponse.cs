namespace Exam.Models;

public class VerifyInvitationResponse
{
    public int CandidateId { get; set; }
    public int ExamId { get; set; }
    public string CandidateName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}