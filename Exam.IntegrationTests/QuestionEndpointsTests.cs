using System.Net;
using System.Net.Http.Json;
using Exam.Models;
using FluentAssertions;
using Xunit;

namespace Exam.IntegrationTests;

public class QuestionEndpointsTests
{
    // ───────────────────────────────────────────
    // CREATE
    // ───────────────────────────────────────────

    [Fact]
    public async Task CreateQuestion_Should_Return_201()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var topicId = await SeedTopicAsync(factory);

        var response = await client.PostAsJsonAsync("/api/questions/", new QuestionRequest
        {
            TopicId = topicId,
            Text = "What is polymorphism?",
            Choices =
            [
                new() { Text = "Ability to take many forms", IsCorrect = true },
                new() { Text = "Encapsulation", IsCorrect = false }
            ]
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await ApiTestHelpers.ReadJsonAsync<QuestionResponse>(response);
        result!.Text.Should().Be("What is polymorphism?");
        result.Choices.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateQuestion_Should_Return_400_When_Text_Empty()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var topicId = await SeedTopicAsync(factory);

        var response = await client.PostAsJsonAsync("/api/questions/", new QuestionRequest
        {
            TopicId = topicId,
            Text = "",
            Choices =
            [
                new() { Text = "A", IsCorrect = true }
            ]
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateQuestion_Should_Return_400_When_No_Correct_Answer()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var topicId = await SeedTopicAsync(factory);

        var response = await client.PostAsJsonAsync("/api/questions/", new QuestionRequest
        {
            TopicId = topicId,
            Text = "What is OOP?",
            Choices =
            [
                new() { Text = "A", IsCorrect = false },
                new() { Text = "B", IsCorrect = false }
            ]
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateQuestion_Should_Return_400_When_Too_Many_Choices()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var topicId = await SeedTopicAsync(factory);

        var response = await client.PostAsJsonAsync("/api/questions/", new QuestionRequest
        {
            TopicId = topicId,
            Text = "What is OOP?",
            Choices =
            [
                new() { Text = "A", IsCorrect = true },
                new() { Text = "B", IsCorrect = false },
                new() { Text = "C", IsCorrect = false },
                new() { Text = "D", IsCorrect = false },
                new() { Text = "E", IsCorrect = false },
                new() { Text = "F", IsCorrect = false },
                new() { Text = "G", IsCorrect = false }  // 7 choices
            ]
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateQuestion_Should_Return_400_When_Duplicate_Choices()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var topicId = await SeedTopicAsync(factory);

        var response = await client.PostAsJsonAsync("/api/questions/", new QuestionRequest
        {
            TopicId = topicId,
            Text = "What is OOP?",
            Choices =
            [
                new() { Text = "Same", IsCorrect = true },
                new() { Text = "Same", IsCorrect = false }  // duplicate
            ]
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateQuestion_Should_Return_400_When_Topic_NotFound()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var response = await client.PostAsJsonAsync("/api/questions/", new QuestionRequest
        {
            TopicId = 99999,
            Text = "What is OOP?",
            Choices =
            [
                new() { Text = "A", IsCorrect = true }
            ]
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ───────────────────────────────────────────
    // GET
    // ───────────────────────────────────────────

    [Fact]
    public async Task GetAllQuestions_Should_Return_200()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var response = await client.GetAsync("/api/questions/");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetQuestionById_Should_Return_200_When_Found()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var questionId = await SeedQuestionAsync(factory, "What is OOP?");

        var response = await client.GetAsync($"/api/questions/{questionId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await ApiTestHelpers.ReadJsonAsync<QuestionResponse>(response);
        result!.Id.Should().Be(questionId);
    }

    [Fact]
    public async Task GetQuestionById_Should_Return_404_When_NotFound()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var response = await client.GetAsync("/api/questions/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetQuestionsByTopic_Should_Return_200_With_Correct_Count()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var topicId = await SeedTopicAsync(factory);

        await SeedQuestionAsync(factory, "Q1", topicId);
        await SeedQuestionAsync(factory, "Q2", topicId);

        var response = await client.GetAsync($"/api/questions/by-topic/{topicId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await ApiTestHelpers.ReadJsonAsync<IEnumerable<QuestionResponse>>(response);
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetPagedQuestions_Should_Return_200()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var response = await client.GetAsync("/api/questions/paged?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await ApiTestHelpers.ReadJsonAsync<PagedResponse<QuestionResponse>>(response);
        result!.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task GetPagedQuestions_Should_Return_400_When_Page_Invalid()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var response = await client.GetAsync("/api/questions/paged?page=0&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ───────────────────────────────────────────
    // UPDATE
    // ───────────────────────────────────────────

    [Fact]
    public async Task UpdateQuestion_Should_Return_200_When_Text_Updated()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var questionId = await SeedQuestionAsync(factory, "Old question");

        // ← PATCH instead of PUT
        var response = await client.PatchAsJsonAsync($"/api/questions/{questionId}",
            new UpdateQuestionRequest { Text = "Updated question text" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await ApiTestHelpers.ReadJsonAsync<QuestionResponse>(response);
        result!.Text.Should().Be("Updated question text");
    }

    [Fact]
    public async Task UpdateQuestion_Should_Keep_Choices_When_Not_Provided()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var questionId = await SeedQuestionAsync(factory, "Old question");

        // Only update text — choices should stay
        var response = await client.PatchAsJsonAsync($"/api/questions/{questionId}",
            new UpdateQuestionRequest { Text = "Updated question text" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await ApiTestHelpers.ReadJsonAsync<QuestionResponse>(response);
        result!.Choices.Should().HaveCount(2);  // original choices preserved
    }

    [Fact]
    public async Task UpdateQuestion_Should_Return_404_When_NotFound()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var response = await client.PatchAsJsonAsync("/api/questions/99999",
            new UpdateQuestionRequest { Text = "Updated" });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


    [Fact]
    public async Task UpdateSpecificChoice_Should_Return_404_When_Choice_NotFound()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var questionId = await SeedQuestionAsync(factory, "Question");

        var response = await client.PatchAsJsonAsync(
            $"/api/questions/{questionId}/choices/99999",
            new ChoiceRequest { Text = "Updated", IsCorrect = true });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ───────────────────────────────────────────
    // DELETE
    // ───────────────────────────────────────────

    [Fact]
    public async Task DeleteQuestion_Should_Return_204()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var questionId = await SeedQuestionAsync(factory, "Delete me");

        var response = await client.DeleteAsync($"/api/questions/{questionId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteQuestion_Should_Return_404_When_NotFound()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var response = await client.DeleteAsync("/api/questions/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ───────────────────────────────────────────
    // SEED HELPERS
    // ───────────────────────────────────────────

    private static async Task<int> SeedTopicAsync(ApiTestApplicationFactory factory)
    {
        var topic = new Topic { Title = $"Topic-{Guid.NewGuid()}" };

        await factory.SeedAsync(async context =>
        {
            context.Topics.Add(topic);
            await context.SaveChangesAsync();  // ← save to get ID
        });

        return topic.Id;
    }

    private static async Task<int> SeedQuestionAsync(
        ApiTestApplicationFactory factory,
        string text,
        int? topicId = null)
    {
        if (!topicId.HasValue)
            topicId = await SeedTopicAsync(factory);

        var question = new Question
        {
            TopicId = topicId.Value,
            Text = text,
            Choices =
            [
                new Choice { Text = "Correct", IsCorrect = true },
                new Choice { Text = "Wrong",   IsCorrect = false }
            ]
        };

        await factory.SeedAsync(async context =>
        {
            context.Questions.Add(question);
            await context.SaveChangesAsync();  // ← save to get ID
        });

        return question.Id;
    }
}