using ExamApi.DTOs.Requests;
using ExamApi.DTOs.Responses;
using ExamApi.Services;

namespace ExamApi.Controllers
{
    public static class QuestionEndpoints
    {
        public static void MapQuestionEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/questions")
                           .WithTags("Questions");
                           

            // GET /api/questions
            group.MapGet("/", async (IQuestionService svc) =>
            {
                var questions = await svc.GetAllAsync();
                return Results.Ok(questions);
            })
            .WithName("GetAllQuestions")
            .WithSummary("Get all questions with their choices")
            .Produces<IEnumerable<QuestionResponse>>(200);

            // GET /api/questions/{id} 
            group.MapGet("/{id:int}", async (int id, IQuestionService svc) =>
            {
                var question = await svc.GetByIdAsync(id);
                return question is null
                    ? Results.NotFound(new { message = $"Question {id} not found." })
                    : Results.Ok(question);
            })
            .WithName("GetQuestionById")
            .WithSummary("Get a single question by ID")
            .Produces<QuestionResponse>(200)
            .Produces(404);

            // GET /api/questions/by-topic/{topicId} 
            group.MapGet("/by-topic/{topicId:int}", async (int topicId, IQuestionService svc) =>
            {
                var questions = await svc.GetByTopicIdAsync(topicId);
                return Results.Ok(questions);
            })
            .WithName("GetQuestionsByTopic")
            .WithSummary("Get all questions under a specific topic")
            .Produces<IEnumerable<QuestionResponse>>(200);

            // POST /api/questions
            group.MapPost("/", async (QuestionRequest request, IQuestionService svc) =>
            {
                var question = await svc.CreateAsync(request);
                return Results.CreatedAtRoute(
                    "GetQuestionById",
                    new { id = question.Id },
                    question);
            })
            .WithName("CreateQuestion")
            .WithSummary("Create a new question")
            .Produces<QuestionResponse>(201)
            .ProducesValidationProblem();

            // PUT /api/questions/{id} 
            group.MapPut("/{id:int}", async (int id, QuestionRequest request, IQuestionService svc) =>
            {
                var question = await svc.UpdateAsync(id, request);
                return question is null
                    ? Results.NotFound(new { message = $"Question {id} not found." })
                    : Results.Ok(question);
            })
            .WithName("UpdateQuestion")
            .WithSummary("Update an existing question")
            .Produces<QuestionResponse>(200)
            .Produces(404)
            .ProducesValidationProblem();

            // DELETE /api/questions/{id}
            group.MapDelete("/{id:int}", async (int id, IQuestionService svc) =>
            {
                var deleted = await svc.DeleteAsync(id);
                return deleted
                    ? Results.NoContent()
                    : Results.NotFound(new { message = $"Question {id} not found." });
            })
            .WithName("DeleteQuestion")
            .WithSummary("Delete a question by ID")
            .Produces(204)
            .Produces(404);
        }
    }

}
