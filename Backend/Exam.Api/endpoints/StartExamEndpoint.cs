namespace Exam.Api;
public static class StartExamEndpoints
{
    public static void MapStartExamEndpoints(this IEndpointRouteBuilder app)
    {
        var exams = app.MapGroup("/exams");

        exams.MapPost("/{examId}/start", async (
            int examId,
            StartExamRequest request,
            IStartExamService service) =>
        {
            // Delegates all business logic to the service layer
            var (response, error) = await service.StartExam(examId, request);

            return response is not null
            ? Results.Ok(response)
            : Results.BadRequest(new { message = error });
        });
    }
}
