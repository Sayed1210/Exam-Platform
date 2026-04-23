namespace ExamApi.DTOs.Responses
{
    public class ChoiceResponse
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public string? ImageUrl { get; set; }
    }

}
