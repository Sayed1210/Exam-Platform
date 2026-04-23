namespace ExamApi.DTOs.Requests
{
    public class CreateExamDto
    {
        public string Title { get; set; } = string.Empty;
        public int DurationMins { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
