using Exam.Models;
using Exam.Repo;
using Exam.Service;
using System.ComponentModel.DataAnnotations;
using Exam.Api.Validation;

namespace Exam.Api;

public static class ExamEndpoints
{
    public static void MapExamManagementEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/exams").WithTags("Exam Management");

        group.MapGet("/", async (IExamService svc) =>
            Results.Ok(await svc.GetAllExamsWithQuestionsAsync()))
            .WithName("GetAllExams")
            .WithSummary("Get all exams")
            .WithDescription("Returns all exams with their assigned questions and choices.")
            .Produces<IEnumerable<ExamResponse>>(200);


        group.MapPost("/", async (
            CreateExamRequest dto,
            IExamService svc,
            IQuestionRepository questionRepo) =>
        {
            var validation = await ValidateCreateOrUpdateAsync(dto, questionRepo);
            if (validation is not null) return validation;

            var exam = await svc.CreateExamAsync(dto);
            return Results.CreatedAtRoute("GetExamWithQuestions", new { id = exam.Id }, exam);
        })
        .WithName("CreateExam")
        .WithSummary("Create a new exam")
        .WithDescription("Creates a new exam. Optionally assign existing questions by providing their IDs.")
        .Produces<ExamResponse>(201)
        .Produces(404)
        .ProducesValidationProblem();


        group.MapMethods("/{id:int}", ["PATCH"], async (
        int id,
     UpdateExamRequest request,
     IExamService svc,
     IQuestionRepository questionRepo) =>
        {
            var validation = EndpointValidation.PositiveNumber(nameof(id), id)
                ?? EndpointValidation.Request(request)
                ?? await ValidateQuestionIdsExistAsync(
                    request.QuestionIds ?? [],
                    nameof(request.QuestionIds),
                    questionRepo);
            if (validation is not null) return validation;

            var exam = await svc.UpdateExamAsync(id, request);
            return exam is null
                ? Results.NotFound(new { message = $"Exam with Id {id} not found" })
                : Results.Ok(exam);
        })
 .WithName("UpdateExam")
 .WithSummary("Partially update an exam")
 .WithDescription("Update title, duration, and/or questions independently. Only provided fields are updated. Send `questionIds: []` to remove all questions, omit `questionIds` to keep existing ones.")
 .Produces<ExamResponse>(200)
 .Produces(404)
 .ProducesValidationProblem();

        group.MapDelete("/{id:int}", async (int id, IExamService svc) =>
        {
            var validation = EndpointValidation.PositiveNumber(nameof(id), id);
            if (validation is not null) return validation;

            var result = await svc.DeleteExamAsync(id);
            return result
                ? Results.NoContent()
                : Results.NotFound(new { message = $"Exam with Id {id} not found" });
        })
        .WithName("DeleteExam")
        .WithSummary("Delete an exam")
        .WithDescription("Permanently deletes an exam by its ID.")
        .Produces(204)
        .Produces(404)
        .ProducesValidationProblem();

        group.MapGet("/{id:int}/questions", async (int id, IExamService svc) =>
        {
            var validation = EndpointValidation.PositiveNumber(nameof(id), id);
            if (validation is not null) return validation;

            var result = await svc.GetExamWithQuestionsAsync(id);
            return result is null
                ? Results.NotFound(new { message = $"Exam {id} not found." })
                : Results.Ok(result);
        })
        .WithName("GetExamWithQuestions")
        .WithSummary("Get exam with questions and choices")
        .WithDescription("Returns a single exam with all its assigned questions and their answer choices.")
        .Produces<ExamResponse>(200)
        .Produces(404)
        .ProducesValidationProblem();

        group.MapPost("/{examId:int}/questions", async (
            int examId,
            AssignQuestionsRequest request,
            IExamService svc,
            IQuestionRepository questionRepo) =>
        {
            var examIdValidation = EndpointValidation.PositiveNumber(nameof(examId), examId);
            if (examIdValidation is not null) return examIdValidation;

            var requestValidation = await ValidateAssignQuestionsAsync(request, questionRepo);
            if (requestValidation is not null) return requestValidation;

            await svc.AssignQuestionsAsync(examId, request.QuestionIds);
            return Results.Ok(new { message = "Questions assigned successfully." });
        })
        .WithName("AssignQuestionsToExam")
        .WithSummary("Assign questions to an exam")
        .WithDescription("Assigns one or more existing questions to an exam. Already assigned questions are ignored.")
        .Produces(200)
        .Produces(404)
        .ProducesValidationProblem();

        group.MapDelete("/{examId:int}/questions/{questionId:int}", async (
            int examId,
            int questionId,
            IExamService svc) =>
        {
            var examIdValidation = EndpointValidation.PositiveNumber(nameof(examId), examId);
            if (examIdValidation is not null) return examIdValidation;

            var questionIdValidation = EndpointValidation.PositiveNumber(nameof(questionId), questionId);
            if (questionIdValidation is not null) return questionIdValidation;

            var result = await svc.RemoveQuestionAsync(examId, questionId);
            return result is null
                ? Results.NotFound(new { message = $"Exam {examId} not found." })
                : Results.NoContent();
        })
        .WithName("RemoveQuestionFromExam")
        .WithSummary("Remove a question from an exam")
        .WithDescription("Unlinks a question from an exam. The question itself is not deleted.")
        .Produces(204)
        .Produces(404)
        .ProducesValidationProblem();
    }

    // Validation helpers

    private static async Task<IResult?> ValidateCreateOrUpdateAsync(
        CreateExamRequest request,
        IQuestionRepository questionRepo)
    {
        var requestValidation = EndpointValidation.Request(request);
        if (requestValidation is not null) return requestValidation;

        return await ValidateQuestionIdsExistAsync(request.QuestionIds, nameof(request.QuestionIds), questionRepo);
    }

    private static async Task<IResult?> ValidateAssignQuestionsAsync(
        AssignQuestionsRequest request,
        IQuestionRepository questionRepo)
    {
        var requestValidation = EndpointValidation.Request(request);
        if (requestValidation is not null) return requestValidation;

        return await ValidateQuestionIdsExistAsync(request.QuestionIds, nameof(request.QuestionIds), questionRepo);
    }

    private static async Task<IResult?> ValidateQuestionIdsExistAsync(
        List<int> questionIds,
        string fieldName,
        IQuestionRepository questionRepo)
    {
        if (questionIds.Count == 0) return null;

        var existingQuestionIds = await questionRepo.GetExistingIdsAsync(questionIds);
        var missingQuestionIds = questionIds.Except(existingQuestionIds).ToList();
        if (missingQuestionIds.Count == 0) return null;

        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            [fieldName] = [$"Questions not found: {string.Join(", ", missingQuestionIds)}."]
        });
    }
}