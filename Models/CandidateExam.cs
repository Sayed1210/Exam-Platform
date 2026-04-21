using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public enum ExamStatus
    {
        PENDING,
        EXPIRED,
        DONE
    }

public class CandidateExam
    {
        public int CandidateId { get; set; }

        public int ExamId { get; set; }

        public float? Score { get; set; }

        public DateTime InvitedAt { get; set; }
        public DateTime? JoinedAt { get; set; }
        public ExamStatus Status { get; set; } = ExamStatus.PENDING;

        // Navigation
        public Candidate? Candidate { get; set; }
        public Exam? Exam { get; set; }
    }
