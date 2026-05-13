using Exam.Models;

public interface IStartExamService
{
    Task<(ExamResponse? Response, string? Error)> StartExam(int examId, StartExamRequest request);
}