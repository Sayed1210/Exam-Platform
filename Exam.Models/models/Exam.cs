using System.ComponentModel.DataAnnotations;
namespace Exam.Models;

public class Exam
{
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public int DurationMins { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation
        public ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();
        public ICollection<CandidateExam> CandidateExams { get; set; } = new List<CandidateExam>();
        public ICollection<CandidateAnswer> CandidateAnswers { get; set; } = new List<CandidateAnswer>();
}