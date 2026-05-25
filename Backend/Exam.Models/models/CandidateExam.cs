using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
namespace Exam.Models;

public enum ExamStatus
{
    PENDING,
    EXPIRED,
    IN_PROGRESS,
    DONE
}

[Index(nameof(InvitationToken), IsUnique = true)]
public class CandidateExam
{
    public int CandidateId { get; set; }

    public int ExamId { get; set; }

    public float? Score { get; set; }

    public DateTime InvitedAt { get; set; }
    public DateTime? JoinedAt { get; set; }
    public ExamStatus Status { get; set; } = ExamStatus.PENDING;

    [Required]
    public required String InvitationToken { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime ExpiryDate { get; set; }

    // Navigation
    public Candidate? Candidate { get; set; }
    public Exam? Exam { get; set; }
}
