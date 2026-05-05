namespace Exam.Models;

public class ExamQuestion
{
    public int ExamId { get; set; }

    public int QuestionId { get; set; }

    // Navigation
    public Exam? Exam { get; set; }
    public Question? Question { get; set; }
}