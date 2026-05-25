namespace Exam.Models;

public class StartExamResponse
{
    public int RemainingSeconds { get; set; }
    public List<CandidateQuestionInExamResponse> Questions { get; set; } = [];
}

public class CandidateQuestionInExamResponse
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public List<CandidateChoiceInExamResponse> Choices { get; set; } = [];
}

public class CandidateChoiceInExamResponse
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}
