using System.ComponentModel.DataAnnotations;

namespace Exam.Models
{
    public class QuestionRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "TopicId must be a positive number.")]
        public int TopicId { get; set; }

        [Required(ErrorMessage = "Question text cannot be empty.")]
        [MinLength(5, ErrorMessage = "Text must be at least 5 characters.")]
        [MaxLength(1000, ErrorMessage = "Question text cannot exceed 1000 characters.")]
        public string Text { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }
        [Required(ErrorMessage = "At least one choice is required.")]
        [MinLength(1, ErrorMessage = "At least one choice is required.")]
        [MaxLength(6, ErrorMessage = "A question cannot have more than 6 choices.")]
        public List<ChoiceRequest> Choices { get; set; } = [];
    }
    public class UpdateQuestionRequest
    {
        [MinLength(5, ErrorMessage = "Text must be at least 5 characters.")]
        [MaxLength(1000, ErrorMessage = "Question text cannot exceed 1000 characters.")]
        public string? Text { get; set; }  // null = don't update

        public string? ImageUrl { get; set; }
         public int TopicId { get; set; }

        public List<ChoiceRequest>? Choices { get; set; }  // null = don't touch, empty = remove all
    }
    public class PaginationRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

}
