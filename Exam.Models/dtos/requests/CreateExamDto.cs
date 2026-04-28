using System.ComponentModel.DataAnnotations;

namespace Exam.Models.dtos.requests
{
    public class CreateExamDto
    {
        public string Title { get; set; } = string.Empty;
        public int DurationMins { get; set; }
       
    }

    public class AssignQuestionsRequest
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one question id is required.")]
        public List<int> QuestionIds { get; set; } = [];
    }
}
