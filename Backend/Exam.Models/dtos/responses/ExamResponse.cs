namespace Exam.Models
{
    public class ExamResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int DurationMins { get; set; }
        public int TotalQuestions { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<QuestionInExamResponse> Questions { get; set; } = [];
    }


    public class QuestionInExamResponse
    {
        public int Id { get; set; }
        public int TopicId { get; set; }
        public string Text { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string TopicTitle { get; set; } = string.Empty;
        public List<ChoiceInExamResponse> Choices { get; set; } = [];
    }

    public class ChoiceInExamResponse
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public string? ImageUrl { get; set; }
    }
}
