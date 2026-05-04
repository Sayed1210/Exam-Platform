using Exam.Models.dtos.requests;
using Exam.Models.dtos.responses;
using Exam.Repo;
using Exam.Service;
using System.ComponentModel.DataAnnotations;

namespace Exam.Api.endpoints;

public static class TopicEndpoints
{
    public static void MapTopicEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/topics").WithTags("Topics");

        group.MapGet("/", async (ITopicService service) =>
            Results.Ok(await service.GetAllTopicsAsync()))
            .WithName("GetAllTopics")
            .WithSummary("Get all topics")
            .WithDescription("Returns all topics with their question count.")
            .Produces<IEnumerable<TopicResponse>>(200);

        group.MapGet("/{id:int}", async (int id, ITopicService service) =>
        {
            var validation = ValidatePositiveNumber(nameof(id), id);
            if (validation is not null) return validation;

            var topic = await service.GetTopicByIdAsync(id);
            return topic == null
                ? Results.NotFound(new { message = $"Topic with Id {id} not found" })
                : Results.Ok(topic);
        })
        .WithName("GetTopicById")
        .WithSummary("Get topic by ID")
        .WithDescription("Returns a single topic by its ID.")
        .Produces<TopicResponse>(200)
        .Produces(404)
        .ProducesValidationProblem();

        group.MapPost("/", async (
            TopicRequest dto,
            ITopicService service,
            ITopicRepository topicRepository) =>
        {
            var validation = await ValidateCreateAsync(dto, topicRepository);
            if (validation is not null) return validation;

            var topic = await service.CreateTopicAsync(dto);
            return Results.Created($"/api/topics/{topic.Id}", topic);
        })
        .WithName("CreateTopic")
        .WithSummary("Create a new topic")
        .WithDescription("Creates a new topic. Title must be unique and cannot exceed 200 characters.")
        .Produces<TopicResponse>(201)
        .ProducesValidationProblem();

        group.MapPut("/{id:int}", async (
            int id,
            TopicRequest dto,
            ITopicService service,
            ITopicRepository topicRepository) =>
        {
            var validation = await ValidateUpdateAsync(id, dto, topicRepository);
            if (validation is not null) return validation;

            var topic = await service.UpdateTopicAsync(id, dto);
            return topic == null
                ? Results.NotFound(new { message = $"Topic with Id {id} not found" })
                : Results.Ok(topic);
        })
        .WithName("UpdateTopic")
        .WithSummary("Update an existing topic")
        .WithDescription("Updates the title of an existing topic. New title must be unique.")
        .Produces<TopicResponse>(200)
        .Produces(404)
        .ProducesValidationProblem();

        group.MapDelete("/{id:int}", async (int id, ITopicService service) =>
        {
            var validation = ValidatePositiveNumber(nameof(id), id);
            if (validation is not null) return validation;

            var result = await service.DeleteTopicAsync(id);
            return !result
                ? Results.NotFound(new { message = $"Topic with Id {id} not found" })
                : Results.NoContent();
        })
        .WithName("DeleteTopic")
        .WithSummary("Delete a topic")
        .WithDescription("Permanently deletes a topic. Cannot delete a topic that has questions linked to it.")
        .Produces(204)
        .Produces(404)
        .ProducesValidationProblem();
    }

    // Validation helpers
    private static IResult? ValidatePositiveNumber(string fieldName, int value)
    {
        if (value > 0) return null;
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            [fieldName] = [$"{fieldName} must be a positive number."]
        });
    }

    private static IResult? Validate<TRequest>(TRequest request)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(request!);
        if (Validator.TryValidateObject(request!, context, results, validateAllProperties: true))
            return null;

        var errors = results
            .GroupBy(result => result.MemberNames.FirstOrDefault() ?? string.Empty)
            .ToDictionary(
                group => group.Key,
                group => group.Select(result => result.ErrorMessage ?? "Invalid value.").ToArray());

        return Results.ValidationProblem(errors);
    }

    private static async Task<IResult?> ValidateCreateAsync(
        TopicRequest request,
        ITopicRepository topicRepository)
    {
        var requestValidation = Validate(request);
        if (requestValidation is not null) return requestValidation;

        return await ValidateDuplicateTitleAsync(request.Title, topicRepository);
    }

    private static async Task<IResult?> ValidateUpdateAsync(
        int id,
        TopicRequest request,
        ITopicRepository topicRepository)
    {
        var idValidation = ValidatePositiveNumber(nameof(id), id);
        if (idValidation is not null) return idValidation;

        var requestValidation = Validate(request);
        if (requestValidation is not null) return requestValidation;

        return await ValidateDuplicateTitleAsync(request.Title, topicRepository, id);
    }

    private static async Task<IResult?> ValidateDuplicateTitleAsync(
        string title,
        ITopicRepository topicRepository,
        int? excludeId = null)
    {
        if (!await topicRepository.TitleExistsAsync(title, excludeId)) return null;

        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            [nameof(TopicRequest.Title)] = [$"Topic with title '{title}' already exists."]
        });
    }
}