using Exam.Models;

public interface IStartExamService
{
    Task<(StartExamResponse? Response, string? Error)> StartExam(int examId, StartExamRequest request);
}