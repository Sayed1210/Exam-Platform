namespace Exam.Models;

public class CandidateResponse
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public float? Score { get; set; }
    public ExamStatus? Status { get; set; }
    public DateTime? InvitedAt { get; set; }
    public DateTime? StartedAt { get; set; }
}
// 1 — Top level
public class CandidateDetailResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public List<CandidateExamDetailResponse> Exams { get; set; } = [];
}

// 2 — Each exam attempt
public class CandidateExamDetailResponse
{
    public string ExamTitle { get; set; } = string.Empty;
    public DateTime InvitedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public ExamStatus Status { get; set; }
    public float? Score { get; set; }
    public List<CandidateAnswerDetail> Answers { get; set; } = [];
}

// 3 — Each answer
public class CandidateAnswerDetail
{
    public string QuestionText { get; set; } = string.Empty;
    public string? QuestionImageUrl { get; set; }
    public string ChoiceText { get; set; } = string.Empty;
    public string? ChoiceImageUrl { get; set; }
    public bool IsCorrect { get; set; }
}