namespace Exam.Models
{
    
    
        public class QuestionResponse
        {
            public int Id { get; set; }
            public int TopicId { get; set; }
            public string TopicTitle { get; set; } = string.Empty;
            public string Text { get; set; } = string.Empty;
            public string? ImageUrl { get; set; }
            public List<ChoiceInQuestion> Choices { get; set; } = [];
        }

        public class ChoiceInQuestion
        {
            
            public string Text { get; set; } = string.Empty;
            public bool IsCorrect { get; set; }
            public string? ImageUrl { get; set; }
        }
    public class PagedResponse<T>
    {
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
