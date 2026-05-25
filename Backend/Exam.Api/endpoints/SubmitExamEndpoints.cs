using Exam.Models;
using Exam.Service;
namespace Exam.Api;
public static class SubmitExamEndpoints
{
    public static void MapSubmitExamEndpoints(this IEndpointRouteBuilder app)
    {
        var exams = app.MapGroup("/exams");

        exams.MapPost("/{examId}/submit", async (
            int examId,
            SubmitExamRequest request,
            IExamSubmissionService service) =>
        {
            // Delegates all business logic to the service layer
            var (success, error) = await service.SubmitExam(examId, request);

            return success
            ? Results.Ok(new { message = "Exam submitted successfully" })
            : Results.BadRequest(new { message = error });
        });

        exams.MapPost("/{examId}/answers", async (
            int examId,
            SaveAnswerRequest request,
            IExamSubmissionService service) =>
        {
            var (success, error) = await service.SaveAnswerAsync(examId, request);

            return success
            ? Results.Ok(new { message = "Answer saved successfully" })
            : Results.BadRequest(new { message = error });
        });

        exams.MapGet("/{examId}/answers/{candidateId}", async (
            int examId,
            int candidateId,
            IExamSubmissionService service) =>
        {
            var (success, error, answers) = await service.GetSavedAnswersAsync(examId, candidateId);

            return success
            ? Results.Ok(new { answers })
            : Results.BadRequest(new { message = error });
        });
    }
}
