using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class Choice
    {
        [Key]
        public int Id { get; set; }

        public int QuestionId { get; set; }

        [Required]
        public string Text { get; set; } = string.Empty;

        [Required]

        public bool IsCorrect { get; set; }
        public string? ImageUrl { get; set; }

        // Navigation
        public Question? Question { get; set; }
        public ICollection<CandidateAnswer> CandidateAnswers { get; set; } = new List<CandidateAnswer>();
    }