using Exam.Models;

public interface IExamSubmissionService
{
    Task SubmitExam(int examId, SubmitExamRequest request);
}
