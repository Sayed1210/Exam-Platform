using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class ExamQuestion
    {
        public int ExamId { get; set; }

        public int QuestionId { get; set; }

        // Navigation
        public Exam? Exam { get; set; }
        public Question? Question { get; set; }
    }