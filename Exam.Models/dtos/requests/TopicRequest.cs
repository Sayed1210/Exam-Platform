using System.ComponentModel.DataAnnotations;

namespace Exam.Models.dtos.requests
{
    public class TopicRequest
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, MinimumLength = 2,
        ErrorMessage = "Title must be between 2 and 100 characters")]
        public string Title { get; set; } = string.Empty;
    }
}
