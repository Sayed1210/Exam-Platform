public class BeforeStartExamResponse
{
    public int CandidateId { get; set; }
    public int ExamId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int DurationMins { get; set; }
    public int TotalQuestions { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string Status { get; set; } = string.Empty;
}
