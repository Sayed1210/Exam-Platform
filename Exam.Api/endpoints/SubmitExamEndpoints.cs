using Exam.Models;
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
    }
}