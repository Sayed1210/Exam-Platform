using System.ComponentModel.DataAnnotations;
namespace Exam.Models;
public class Question
{
    [Key]
    public int Id { get; set; }

    public int TopicId { get; set; }

    [Required]
    public string Text { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    // Navigation
    public Topic? Topic { get; set; }
    public ICollection<Choice> Choices { get; set; } = new List<Choice>();
    public ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();
    public ICollection<CandidateAnswer> CandidateAnswers { get; set; } = new List<CandidateAnswer>();
}