using System.Net;
using System.Net.Http.Json;
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

        var response = await client.PostAsJsonAsync("/api/exams/", new CreateExamRequest
        {
            Title = "Test Exam",
            DurationMins = 60,
            QuestionIds = []
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await ApiTestHelpers.ReadJsonAsync<ExamResponse>(response);
        result!.Title.Should().Be("Test Exam");
        result.DurationMins.Should().Be(60);
    }

    [Fact]
    public async Task CreateExam_Should_Return_400_When_Title_Empty()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var response = await client.PostAsJsonAsync("/api/exams/", new CreateExamRequest
        {
            Title = "",
            DurationMins = 60,
            QuestionIds = []
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateExam_Should_Return_400_When_Duration_Invalid()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var response = await client.PostAsJsonAsync("/api/exams/", new CreateExamRequest
        {
            Title = "Test",
            DurationMins = 0,
            QuestionIds = []
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
    }

  

    [Fact]
    public async Task UpdateExam_Should_Return_200_When_Title_Updated()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var examId = await SeedExamAsync(factory);

        // ← PATCH instead of PUT
        var response = await client.PatchAsJsonAsync($"/api/exams/{examId}",
            new UpdateExamRequest { Title = "Updated Title" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await ApiTestHelpers.ReadJsonAsync<ExamResponse>(response);
        result!.Title.Should().Be("Updated Title");
    }

    [Fact]
    public async Task UpdateExam_Should_Return_200_When_Duration_Updated()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var examId = await SeedExamAsync(factory);

        var response = await client.PatchAsJsonAsync($"/api/exams/{examId}",
            new UpdateExamRequest { DurationMins = 90 });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await ApiTestHelpers.ReadJsonAsync<ExamResponse>(response);
        result!.DurationMins.Should().Be(90);
    }

    [Fact]
    public async Task UpdateExam_Should_Return_404_When_NotFound()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var response = await client.PatchAsJsonAsync($"/api/exams/99999",
            new UpdateExamRequest { Title = "New" });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteExam_Should_Return_204()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var examId = await SeedExamAsync(factory);

        var response = await client.DeleteAsync($"/api/exams/{examId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteExam_Should_Return_404_When_NotFound()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var response = await client.DeleteAsync("/api/exams/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AssignQuestions_Should_Return_200()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var examId = await SeedExamAsync(factory);
        var questionIds = await SeedQuestionsAsync(factory, 2);

        var response = await client.PostAsJsonAsync($"/api/exams/{examId}/questions",
            new AssignQuestionsRequest { QuestionIds = questionIds });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AssignQuestions_Should_Return_400_When_QuestionIds_Empty()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var examId = await SeedExamAsync(factory);

        var response = await client.PostAsJsonAsync($"/api/exams/{examId}/questions",
            new AssignQuestionsRequest { QuestionIds = [] });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetExamWithQuestions_Should_Return_200_With_Questions()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var examId = await SeedExamAsync(factory);
        var questionIds = await SeedQuestionsAsync(factory, 2);

        await factory.SeedAsync(async context =>
        {
            context.ExamQuestions.AddRange(questionIds.Select(qId => new ExamQuestion
            {
                ExamId = examId,
                QuestionId = qId
            }));
            await context.SaveChangesAsync();
        });

        var response = await client.GetAsync($"/api/exams/{examId}/questions");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await ApiTestHelpers.ReadJsonAsync<ExamResponse>(response);
        result!.Questions.Should().HaveCount(2);
    }

    [Fact]
    public async Task RemoveQuestion_Should_Return_204()
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
            await context.SaveChangesAsync();
        });

        var response = await client.DeleteAsync($"/api/exams/{examId}/questions/{questionIds[0]}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task RemoveQuestion_Should_Return_404_When_Exam_NotFound()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var response = await client.DeleteAsync("/api/exams/99999/questions/1");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ─────────────────────────────────────────────
    // SEED HELPERS
    // ─────────────────────────────────────────────

    private static async Task<int> SeedExamAsync(ApiTestApplicationFactory factory)
    {
        var exam = new Exam.Models.Exam
        {
            Title = "Seeded Exam",
            DurationMins = 45
        };

        await factory.SeedAsync(async context =>
        {
            context.Exams.Add(exam);
            await context.SaveChangesAsync();
        });

        return exam.Id;
    }

    private static async Task<List<int>> SeedQuestionsAsync(
        ApiTestApplicationFactory factory, int count)
    {
        var ids = new List<int>();

        await factory.SeedAsync(async context =>
        {
            var topic = new Topic { Title = Guid.NewGuid().ToString() };
            context.Topics.Add(topic);
            await context.SaveChangesAsync();

            for (int i = 0; i < count; i++)
            {
                var q = new Question
                {
                    TopicId = topic.Id,
                    Text = $"Question {i + 1}",
                    Choices =
                    {
                        new Choice { Text = "A", IsCorrect = true }
                    }
                };
                context.Questions.Add(q);
                await context.SaveChangesAsync();
                ids.Add(q.Id);
            }
        });

        return ids;
    }
}