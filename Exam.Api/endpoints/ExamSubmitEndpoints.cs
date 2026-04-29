using Exam.Models;
public static class ExamEndpoints
{
    public static void MapExamEndpoints(this IEndpointRouteBuilder app)
    {
        var exams = app.MapGroup("/exams");

        exams.MapPost("/{examId}/submit", async (
            int examId,
            SubmitExamRequest request,
            IExamSubmissionService service) =>
        {
            // Delegates all business logic to the service layer
            await service.SubmitExam(examId, request);

            // Returns a simple success response (HTTP 200 OK)
            return Results.Ok(new
            {
                message = "Exam submitted successfully"
            });
        });
    }
}