using System.ComponentModel.DataAnnotations;
namespace Exam.Models;

public class Choice
{
    [Key]
    public int Id { get; set; }

    public int QuestionId { get; set; }

    public string? Text { get; set; }

    [Required]
    public bool IsCorrect { get; set; }
    public string? ImageUrl { get; set; }

    // Navigation
    public Question? Question { get; set; }
    public ICollection<CandidateAnswer> CandidateAnswers { get; set; } = new List<CandidateAnswer>();
}