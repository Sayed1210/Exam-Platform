using System.ComponentModel.DataAnnotations;
namespace Exam.Models;

public class Topic
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        // Navigation
        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }