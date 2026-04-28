using Exam.Models.dtos.requests;
using Exam.Models.dtos.responses;
using Exam.Service;
namespace Exam.Api.endpoints
{
    public static class ChoiceEndpoints
    {
        public static void MapChoiceEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/choices")
                           .WithTags("Choices");
                           

            // ── GET /api/choices/by-question/{questionId} 
            group.MapGet("/by-question/{questionId:int}", async (
                int questionId,
                IChoiceService svc) =>
            {
                var choices = await svc.GetAllByQuestionIdAsync(questionId);
                return Results.Ok(choices);
            })
            .WithName("GetChoicesByQuestion")
            .WithSummary("Get all choices for a specific question")
            .Produces<IEnumerable<ChoiceResponse>>(200);

            // ── GET /api/choices/{id} 
            group.MapGet("/{id:int}", async (int id, IChoiceService svc) =>
            {
                var choice = await svc.GetByIdAsync(id);
                return choice is null
                    ? Results.NotFound(new { message = $"Choice {id} not found." })
                    : Results.Ok(choice);
            })
            .WithName("GetChoiceById")
            .WithSummary("Get a single choice by ID")
            .Produces<ChoiceResponse>(200)
            .Produces(404);

            // ── POST /api/choices 
            group.MapPost("/", async (ChoiceRequest request, IChoiceService svc) =>
            {
                var choice = await svc.CreateAsync(request);
                return Results.CreatedAtRoute(
                    "GetChoiceById",
                    new { id = choice.Id },
                    choice);
            })
            .WithName("CreateChoice")
            .WithSummary("Create a new choice for a question")
            .Produces<ChoiceResponse>(201)
            .ProducesValidationProblem();

            // ── PUT /api/choices/{id}
            group.MapPut("/{id:int}", async (int id, ChoiceRequest request, IChoiceService svc) =>
            {
                var choice = await svc.UpdateAsync(id, request);
                return choice is null
                    ? Results.NotFound(new { message = $"Choice {id} not found." })
                    : Results.Ok(choice);
            })
            .WithName("UpdateChoice")
            .WithSummary("Update an existing choice")
            .Produces<ChoiceResponse>(200)
            .Produces(404)
            .ProducesValidationProblem();

            // ── DELETE /api/choices/{id} 
            group.MapDelete("/{id:int}", async (int id, IChoiceService svc) =>
            {
                var deleted = await svc.DeleteAsync(id);
                return deleted
                    ? Results.NoContent()
                    : Results.NotFound(new { message = $"Choice {id} not found." });
            })
            .WithName("DeleteChoice")
            .WithSummary("Delete a choice by ID")
            .Produces(204)
            .Produces(404);
        }
    }
}
