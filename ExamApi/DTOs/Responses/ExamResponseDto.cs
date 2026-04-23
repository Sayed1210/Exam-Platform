namespace ExamApi.DTOs.Responses
{
    public class ExamResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int DurationMins { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

    }
}
