using Exam.Models;
namespace Exam.Service;

public interface IExamSubmissionService
{
    Task<(bool Success, string? Error)> SubmitExam(int examId, SubmitExamRequest request);
    Task<(bool Success, string? Error)> SaveAnswerAsync(int examId, SaveAnswerRequest request);
    Task<(bool Success, string? Error, List<AnswerRequest> Answers)> GetSavedAnswersAsync(int examId, int candidateId);
}
