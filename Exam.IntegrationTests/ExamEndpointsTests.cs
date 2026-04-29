using System.Net;
using System.Net.Http.Json;
using Exam.Data;
using Exam.Models;
using Exam.Models.dtos.requests;
using Exam.Models.dtos.responses;
using FluentAssertions;
using Xunit;

namespace Exam.IntegrationTests;

public class ExamEndpointsTests
{
    [Fact]
    public async Task CreateExam_Should_Return_201()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var response = await client.PostAsJsonAsync("/api/exams/", new CreateExamDto
        {
            Title = "Test Exam",
            DurationMins = 60
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await ApiTestHelpers.ReadJsonAsync<ExamResponseDto>(response);
        result.Should().NotBeNull();
        result!.Title.Should().Be("Test Exam");
    }

    [Fact]
    public async Task CreateExam_Should_Return_400_When_Title_Empty()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var response = await client.PostAsJsonAsync("/api/exams/", new CreateExamDto
        {
            Title = "",
            DurationMins = 60
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetAllExams_Should_Return_200()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var response = await client.GetAsync("/api/exams/");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ApiTestHelpers.ReadJsonAsync<IEnumerable<ExamResponseDto>>(response);
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetExamById_Should_Return_404_When_Not_Found()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var response = await client.GetAsync("/api/exams/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetExamById_Should_Return_200_When_Found()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var createResponse = await client.PostAsJsonAsync("/api/exams/", new CreateExamDto
        {
            Title = "Find Me",
            DurationMins = 30
        });
        var created = await ApiTestHelpers.ReadJsonAsync<ExamResponseDto>(createResponse);

        var response = await client.GetAsync($"/api/exams/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ApiTestHelpers.ReadJsonAsync<ExamResponseDto>(response);
        result!.Title.Should().Be("Find Me");
    }

    [Fact]
    public async Task UpdateExam_Should_Return_200()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var createResponse = await client.PostAsJsonAsync("/api/exams/", new CreateExamDto
        {
            Title = "Old Title",
            DurationMins = 30
        });
        var created = await ApiTestHelpers.ReadJsonAsync<ExamResponseDto>(createResponse);

        var response = await client.PutAsJsonAsync($"/api/exams/{created!.Id}", new CreateExamDto
        {
            Title = "New Title",
            DurationMins = 60
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ApiTestHelpers.ReadJsonAsync<ExamResponseDto>(response);
        result!.Title.Should().Be("New Title");
    }

    [Fact]
    public async Task UpdateExam_Should_Return_404_When_Not_Found()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var response = await client.PutAsJsonAsync("/api/exams/99999", new CreateExamDto
        {
            Title = "New Title",
            DurationMins = 60
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteExam_Should_Return_204()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var createResponse = await client.PostAsJsonAsync("/api/exams/", new CreateExamDto
        {
            Title = "Delete Me",
            DurationMins = 30
        });
        var created = await ApiTestHelpers.ReadJsonAsync<ExamResponseDto>(createResponse);

        var response = await client.DeleteAsync($"/api/exams/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteExam_Should_Return_404_When_Not_Found()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var response = await client.DeleteAsync("/api/exams/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AssignQuestions_Should_Return_200_When_Questions_Are_Assigned()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var examId = await SeedExamAsync(factory);
        var questionIds = await SeedQuestionsAsync(factory, 2);

        var response = await client.PostAsJsonAsync($"/api/exams/{examId}/questions", new AssignQuestionsRequest
        {
            QuestionIds = questionIds
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetExamQuestions_Should_Return_200_With_Assigned_Questions()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var examId = await SeedExamAsync(factory);
        var questionIds = await SeedQuestionsAsync(factory, 1);

        await factory.SeedAsync(async context =>
        {
            context.ExamQuestions.Add(new ExamQuestion
            {
                ExamId = examId,
                QuestionId = questionIds[0]
            });
            await Task.CompletedTask;
        });

        var response = await client.GetAsync($"/api/exams/{examId}/questions");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ApiTestHelpers.ReadJsonAsync<ExamResponseDto>(response);
        result!.Questions.Should().ContainSingle();
    }

    [Fact]
    public async Task RemoveQuestionFromExam_Should_Return_204()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var examId = await SeedExamAsync(factory);
        var questionIds = await SeedQuestionsAsync(factory, 1);

        await factory.SeedAsync(async context =>
        {
            context.ExamQuestions.Add(new ExamQuestion
            {
                ExamId = examId,
                QuestionId = questionIds[0]
            });
            await Task.CompletedTask;
        });

        var response = await client.DeleteAsync($"/api/exams/{examId}/questions/{questionIds[0]}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private static async Task<int> SeedExamAsync(ApiTestApplicationFactory factory)
    {
        var exam = new Exam.Models.Exam
        {
            Title = "Seeded exam",
            DurationMins = 45
        };

        await factory.SeedAsync(async context =>
        {
            context.Exams.Add(exam);
            await Task.CompletedTask;
        });

        return exam.Id;
    }

    private static async Task<List<int>> SeedQuestionsAsync(ApiTestApplicationFactory factory, int count)
    {
        var ids = new List<int>();

        await factory.SeedAsync(async context =>
        {
            var topic = new Topic { Title = $"Topic-{Guid.NewGuid()}" };
            context.Topics.Add(topic);
            await context.SaveChangesAsync();

            for (var index = 0; index < count; index++)
            {
                var question = new Question
                {
                    TopicId = topic.Id,
                    Text = $"Question {index + 1}",
                    Choices =
                    {
                        new Choice { Text = $"Choice {index + 1}", IsCorrect = true }
                    }
                };
                context.Questions.Add(question);
                await context.SaveChangesAsync();
                ids.Add(question.Id);
            }
        });

        return ids;
    }
}
