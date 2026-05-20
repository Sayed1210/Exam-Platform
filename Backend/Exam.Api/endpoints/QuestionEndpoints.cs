using Exam.Models;
using Exam.Repo;
using Exam.Service;
using System.ComponentModel.DataAnnotations;
using Exam.Api.Validation;

namespace Exam.Api;

public static class QuestionEndpoints
{
    public static void MapQuestionEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/questions").WithTags("Questions");

group.MapGet("/", async (
    IQuestionService svc,
    int page = 1,
    int pageSize = 10,
    string? search = null,
    int[]? topicId = null) =>
{
    var result = await svc.GetAllAsync(page, pageSize, search, topicId);
    return Results.Ok(result);
})
.WithName("GetAllQuestions")
.WithSummary("Get all questions")
.WithDescription("Returns paginated questions with optional search and topic filter.")
.Produces<PagedResponse<QuestionResponse>>(200);



        group.MapGet("/{id:int}", async (int id, IQuestionService svc) =>
        {
            var idValidation = EndpointValidation.PositiveNumber(nameof(id), id);
            if (idValidation is not null) return idValidation;

            var question = await svc.GetByIdAsync(id);
            return question is null
                ? Results.NotFound(new { message = $"Question {id} not found." })
                : Results.Ok(question);
        })
        .WithName("GetQuestionById")
        .WithSummary("Get question by ID")
        .WithDescription("Returns a single question with its topic and answer choices.")
        .Produces<QuestionResponse>(200)
        .Produces(404)
        .ProducesValidationProblem();

        group.MapGet("/by-topic/{topicId:int}", async (int topicId, IQuestionService svc) =>
        {
            var idValidation = EndpointValidation.PositiveNumber(nameof(topicId), topicId);
            if (idValidation is not null) return idValidation;

            return Results.Ok(await svc.GetByTopicIdAsync(topicId));
        })
        .WithName("GetQuestionsByTopic")
        .WithSummary("Get questions by topic")
        .WithDescription("Returns all questions that belong to a specific topic.")
        .Produces<IEnumerable<QuestionResponse>>(200)
        .ProducesValidationProblem();

        group.MapPost("/", async (
            QuestionRequest request,
            IQuestionService svc,
            ITopicRepository topicRepo) =>
        {
            var validation = await ValidateCreateAsync(request, topicRepo);
            if (validation is not null) return validation;

            var question = await svc.CreateAsync(request);
            return Results.CreatedAtRoute("GetQuestionById", new { id = question.Id }, question);
        })
        .WithName("CreateQuestion")
        .WithSummary("Create a new question")
        .WithDescription("Creates a new question under a topic. Optionally include up to 6 answer choices.")
        .Produces<QuestionResponse>(201)
        .Produces(404)
        .ProducesValidationProblem();


        // PATCH /api/questions/{questionId}/choices/{choiceId}
        group.MapMethods("/{questionId:int}/choices/{choiceId:int}", ["PATCH"], async (
            int questionId,
            int choiceId,
            ChoiceRequest request,
            IQuestionService svc) =>
        {
            var validation = EndpointValidation.PositiveNumber(nameof(questionId), questionId)
                ?? EndpointValidation.PositiveNumber(nameof(choiceId), choiceId)
                ?? EndpointValidation.Request(request);
            if (validation is not null) return validation;

            var question = await svc.UpdateChoiceAsync(questionId, choiceId, request);

            if (question is null)
                return Results.NotFound(new { message = $"Question {questionId} or Choice {choiceId} not found." });

            return Results.Ok(question);
        })
        .WithName("UpdateSpecificChoice")
        .WithSummary("Update a specific choice")
        .WithDescription("Updates text, isCorrect, and imageUrl of a specific choice by its ID.")
        .Produces<QuestionResponse>(200)
        .Produces(404)
        .ProducesValidationProblem();


        group.MapMethods("/{id:int}", ["PATCH"], async (
            int id,
            UpdateQuestionRequest request,
            IQuestionService svc) =>
        {
            var validation = EndpointValidation.PositiveNumber(nameof(id), id)
                ?? EndpointValidation.Request(request)
                ?? ValidateChoices(request.Choices);
            if (validation is not null) return validation;

            var question = await svc.UpdateAsync(id, request);
            return question is null
                ? Results.NotFound(new { message = $"Question {id} not found." })
                : Results.Ok(question);
        })
        .WithName("UpdateQuestion")
        .WithSummary("Partially update a question")
        .WithDescription("Update text, imageUrl, and/or choices independently. Topic cannot be changed. Omit choices to keep existing ones.")
        .Produces<QuestionResponse>(200)
        .Produces(404)
        .ProducesValidationProblem();

        group.MapDelete("/{id:int}", async (int id, IQuestionService svc) =>
        {
            var idValidation = EndpointValidation.PositiveNumber(nameof(id), id);
            if (idValidation is not null) return idValidation;

            var deleted = await svc.DeleteAsync(id);
            return deleted
                ? Results.NoContent()
                : Results.NotFound(new { message = $"Question {id} not found." });
        })
        .WithName("DeleteQuestion")
        .WithSummary("Delete a question")
        .WithDescription("Permanently deletes a question and all its associated choices.")
        .Produces(204)
        .Produces(404)
        .ProducesValidationProblem();
    }

    // ── VALIDATION HELPERS ────────────────────────────────────────────

    private static async Task<IResult?> ValidateCreateAsync(
        QuestionRequest request,
        ITopicRepository topicRepo)
    {
        var requestValidation = EndpointValidation.Request(request);
        if (requestValidation is not null) return requestValidation;

        var choicesValidation = ValidateChoices(request.Choices);
        if (choicesValidation is not null) return choicesValidation;

        return await ValidateTopicExistsAsync(request.TopicId, topicRepo);
    }

   private static IResult? ValidateChoices(
    List<ChoiceRequest>? choices)
{
    if (choices is null || choices.Count == 0)
        return null;

    if (choices.Count < 2)
    {
        return Results.ValidationProblem(
            new Dictionary<string, string[]>
            {
                ["Choices"] =
                [
                    "A question cannot have less than 2 choices."
                ]
            });
    }

    foreach (var choice in choices)
    {
        var hasText =
            !string.IsNullOrWhiteSpace(choice.Text);

        var hasImage =
            !string.IsNullOrWhiteSpace(choice.ImageUrl);

        // must have exactly one
        if (hasText == hasImage)
        {
            return Results.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    ["Choices"] =
                    [
                        "Each choice must contain either text or image, but not both."
                    ]
                });
        }
    }

    if (!choices.Any(c => c.IsCorrect))
    {
        return Results.ValidationProblem(
            new Dictionary<string, string[]>
            {
                ["Choices"] =
                [
                    "At least one choice must be marked as correct."
                ]
            });
    }

    // duplicate TEXT choices only
    var textChoices = choices
        .Where(c => !string.IsNullOrWhiteSpace(c.Text))
        .Select(c => c.Text!.Trim().ToLower());

    if (textChoices.GroupBy(x => x)
        .Any(g => g.Count() > 1))
    {
        return Results.ValidationProblem(
            new Dictionary<string, string[]>
            {
                ["Choices"] =
                [
                    "Duplicate choice texts are not allowed."
                ]
            });
    }

    return null;
}

    private static async Task<IResult?> ValidateTopicExistsAsync(
        int topicId,
        ITopicRepository topicRepo)
    {
        var exists = await topicRepo.ExistsAsync(topicId);
        if (exists) return null;

        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["TopicId"] = [$"Topic with id {topicId} not found."]
        });
    }
}