using System.ComponentModel.DataAnnotations;

namespace Exam.Models.dtos.requests
{
    public class ChoiceRequest
    {
        [Required]
        public int QuestionId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Choice text cannot be empty.")]
        public string Text { get; set; } = string.Empty;

        [Required]
        public bool IsCorrect { get; set; }

        public string? ImageUrl { get; set; }
    }
}
