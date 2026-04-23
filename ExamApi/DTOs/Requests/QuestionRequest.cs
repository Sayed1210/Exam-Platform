using System.ComponentModel.DataAnnotations;

namespace ExamApi.DTOs.Requests
{
    public class QuestionRequest
    {
        [Required]
        public int TopicId { get; set; }

        [Required]
        [MinLength(5, ErrorMessage = "Text must be at least 5 characters.")]
        public string Text { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }
    }
}
