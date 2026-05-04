using System.ComponentModel.DataAnnotations;

namespace Exam.Models
{
    public class CreateExamRequest : IValidatableObject
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string Title { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Duration must be greater than 0 minutes.")]
        public int DurationMins { get; set; }

        public List<int> QuestionIds { get; set; } = [];

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
           

            var duplicateIds = QuestionIds
                .GroupBy(id => id)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();

            if (duplicateIds.Count > 0)
            {
                yield return new ValidationResult(
                    $"Duplicate question ids: {string.Join(", ", duplicateIds)}.",
                    [nameof(QuestionIds)]);
            }

            if (QuestionIds.Any(id => id <= 0))
            {
                yield return new ValidationResult(
                    "All question ids must be positive numbers.",
                    [nameof(QuestionIds)]);
            }
        }
    }
    public class UpdateExamRequest
    {
        [MaxLength(200, ErrorMessage = "Exam title cannot exceed 200 characters.")]
        public string? Title { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Duration must be greater than 0 minutes.")]
        public int? DurationMins { get; set; }

        public List<int>? QuestionIds { get; set; }  // null = don't touch, empty = remove all
    }

    public class AssignQuestionsRequest : IValidatableObject
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one question id is required.")]
        public List<int> QuestionIds { get; set; } = [];

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var duplicateIds = QuestionIds
                .GroupBy(id => id)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();

            if (duplicateIds.Count > 0)
            {
                yield return new ValidationResult(
                    $"Duplicate question ids: {string.Join(", ", duplicateIds)}.",
                    [nameof(QuestionIds)]);
            }

            if (QuestionIds.Any(id => id <= 0))
            {
                yield return new ValidationResult(
                    "All question ids must be positive numbers.",
                    [nameof(QuestionIds)]);
            }
        }
    }
}