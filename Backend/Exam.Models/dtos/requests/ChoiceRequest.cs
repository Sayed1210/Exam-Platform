using System.ComponentModel.DataAnnotations;

namespace Exam.Models
{
    public class ChoiceRequest : IValidatableObject
    {
        [MaxLength(500,
            ErrorMessage =
                "Choice text cannot exceed 500 characters.")]
        public string? Text { get; set; }

        public bool IsCorrect { get; set; }

        public string? ImageUrl { get; set; }

        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            var hasText =
                !string.IsNullOrWhiteSpace(Text);

            var hasImage =
                !string.IsNullOrWhiteSpace(ImageUrl);

            if (hasText == hasImage)
            {
                yield return new ValidationResult(
                    "A choice must contain exactly one of text or image.",
                    new[]
                    {
                        nameof(Text),
                        nameof(ImageUrl)
                    });
            }
        }
    }
}