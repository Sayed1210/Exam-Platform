using System.ComponentModel.DataAnnotations;

namespace Exam.Models
{
    public class ChoiceRequest
    {
     

        [Required(ErrorMessage = "Choice text cannot be empty.")]
        [MaxLength(500, ErrorMessage = "Choice text cannot exceed 500 characters.")]
        public string Text { get; set; } = string.Empty;

        public bool IsCorrect { get; set; }
        public string? ImageUrl { get; set; }
    }
}