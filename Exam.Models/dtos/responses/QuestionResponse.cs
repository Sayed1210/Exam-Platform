namespace Exam.Models.dtos.responses
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
            public int Id { get; set; }
            public string Text { get; set; } = string.Empty;
            public bool IsCorrect { get; set; }
            public string? ImageUrl { get; set; }
        }
    }
