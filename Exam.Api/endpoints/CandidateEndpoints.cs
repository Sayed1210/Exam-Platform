using Exam.Services;

// Static class used to group endpoint definitions
// This is a common pattern in Minimal APIs to keep Program.cs clean
public static class CandidateEndpoints
{
    // Extension method to register all candidate-related endpoints
    public static void MapCandidateEndpoints(this IEndpointRouteBuilder app)
    {
        var candidates = app.MapGroup("/candidates");

        candidates.MapGet("/", async (ICandidateService service) =>
        {
            var result = await service.GetAllCandidates();
            return Results.Ok(result);
        });

        candidates.MapGet("/{id}", async (int id, ICandidateService service) =>
        {
            var candidate = await service.GetCandidateById(id);

            if (candidate is null)
                return Results.NotFound();

            return Results.Ok(candidate);
        });

        candidates.MapGet("/exam/{examId}", async (int examId, ICandidateService service) =>
        {
            var candidate = await service.GetCandidateByExamId(examId);

            if (candidate is null)
                return Results.NotFound();

            return Results.Ok(candidate);
        });

        candidates.MapPost("/", async (CreateCandidate dto, ICandidateService service) =>
        {
            await service.AddCandidate(dto);

            return Results.Ok(new { message = "Candidate added successfully" });
        });

        candidates.MapDelete("/{id}", async (int id, ICandidateService service) =>
        {
            await service.DeleteCandidate(id);
            return Results.Ok(new { message = "Candidate deleted successfully" });
        });
    }
}
