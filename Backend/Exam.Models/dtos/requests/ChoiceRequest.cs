using System.ComponentModel.DataAnnotations;

namespace Exam.Models
{
    public class ChoiceRequest 
    {
        [MaxLength(500,
            ErrorMessage =
                "Choice text cannot exceed 500 characters.")]
        public string? Text { get; set; }

        public bool IsCorrect { get; set; }

        public string? ImageUrl { get; set; }

    }
}