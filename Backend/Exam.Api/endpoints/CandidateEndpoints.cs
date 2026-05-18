using System.ComponentModel;
using Exam.Service;
using Exam.Models;
// Static class used to group endpoint definitions
// This is a common pattern in Minimal APIs to keep Program.cs clean
public static class CandidateEndpoints
{
    // Extension method to register all candidate-related endpoints
    public static void MapCandidateEndpoints(this IEndpointRouteBuilder app)
    {
        var candidates = app.MapGroup("/candidates");

       
        candidates.MapGet("/", async (
            ICandidateService service,
            int page = 1,
            int pageSize = 8,
            string? search = null,
            int? status = null,
            bool noStatus = false) =>
        {
            var result = await service.GetAllCandidates(page, pageSize, search, status, noStatus);
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

        candidates.MapPost("/", async (CreateCandidateRequest dto, ICandidateService service) =>
        {
            bool added = await service.AddCandidate(dto);
            if (!added)
                return Results.BadRequest(new { message = "Candidate already exists" });

            return Results.Ok(new { message = "Candidate added successfully" });
        });

        candidates.MapGet("/{id:int}/details", async (int id, ICandidateService svc) =>
        {

            var result = await svc.GetDetailAsync(id);
            return result is null
                ? Results.NotFound(new { message = $"Candidate {id} not found." })
                : Results.Ok(result);
        })
        .WithName("GetCandidateDetail")
        .WithSummary("Get candidate full details")
        .WithDescription("Returns candidate info with all exam attempts and answers. Returns empty exams array if not assigned to any exam.")
        .Produces<CandidateDetailResponse>(200)
        .Produces(404)
        .ProducesValidationProblem();

        candidates.MapGet("/unassigned", async (ICandidateService service) =>
        {
            var result = await service.GetUnassignedCandidates();

            return Results.Ok(result); // recommended: always return 200 even if empty
        });

        candidates.MapDelete("/{id}", async (int id, ICandidateService service) =>
        {
            bool exists = await service.DeleteCandidate(id);
            if (!exists)
                return Results.BadRequest(new { message = "Candidate doesn't exist" });

            return Results.Ok(new { message = "Candidate deleted successfully" });
        });
    }
}
