using Exam.Models;

public interface IExamSubmissionService
{
    Task<(bool Success, string? Error)> SubmitExam(int examId, SubmitExamRequest request);
}
