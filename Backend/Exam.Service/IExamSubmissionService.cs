using Exam.Models;
namespace Exam.Service;

public interface IExamSubmissionService
{
    Task<(bool Success, string? Error)> SubmitExam(int examId, SubmitExamRequest request);
}
