namespace Exam.Models;
public class CandidateAnswer
{
    public int CandidateId { get; set; }

    public int QuestionId { get; set; }

    public int ExamId { get; set; }

    public int ChoiceId { get; set; }

    // Navigation
    public Candidate? Candidate { get; set; }
    public Question? Question { get; set; }
    public Exam? Exam { get; set; }
    public Choice? Choice { get; set; }
}