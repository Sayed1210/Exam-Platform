public class SubmitExamRequest
{
    public int CandidateId { get; set; }
    public List<AnswerRequest> Answers { get; set; } = [];
}

public class AnswerRequest
{
    public int QuestionId { get; set; }
    public int ChoiceId { get; set; }
}
