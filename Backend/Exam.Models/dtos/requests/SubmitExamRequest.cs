namespace Exam.Models;

public class SubmitExamRequest
{
    public int CandidateId { get; set; }
    public List<AnswerRequest> Answers { get; set; } = [];
}

public class SaveAnswerRequest
{
    public int CandidateId { get; set; }
    public int QuestionId { get; set; }
    public int ChoiceId { get; set; }
}

public class AnswerRequest
{
    public int QuestionId { get; set; }
    public int ChoiceId { get; set; }
}
