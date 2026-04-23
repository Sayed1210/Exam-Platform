namespace ExamApi.DTOs.Responses
{
    public class TopicResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int QuestionsCount { get; set; }
    }
}
