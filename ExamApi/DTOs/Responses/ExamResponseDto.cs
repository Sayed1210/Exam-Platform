namespace ExamApi.DTOs.Responses
{
    public class ExamResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int DurationMins { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int TotalQuestions { get; set; }
        public List<QuestionInExamDto> Questions { get; set; } = [];
    }


    public class QuestionInExamDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public List<ChoiceInExamDto> Choices { get; set; } = [];
    }

    public class ChoiceInExamDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public string? ImageUrl { get; set; }
    }
}
