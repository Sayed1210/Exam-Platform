namespace Exam.Api;

public static class BeforeStartExamEndpoints
{
    public static void MapBeforeStartExamEndpoints(this IEndpointRouteBuilder app)
    {
        var exams = app.MapGroup("/exams");

        exams.MapGet("/before-start", async (
            string token,
            IBeforeStartExamService service) =>
        {
            var (response, error) = await service.GetExamInfo(token);

            return response is not null
                ? Results.Ok(response)
                : Results.BadRequest(new { message = error });
        });
    }
}