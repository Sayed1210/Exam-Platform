using System.Net;
using System.Net.Http.Json;
using Exam.Models;
using Exam.Models.dtos.requests;
using Exam.Models.dtos.responses;
using FluentAssertions;
using Xunit;

namespace Exam.IntegrationTests;

public class ChoiceEndpointsTests
{
    [Fact]
    public async Task CreateChoice_Should_Return_201()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);
        var questionId = await SeedQuestionAsync(factory);

        var response = await client.PostAsJsonAsync("/api/choices/", new ChoiceRequest
        {
            QuestionId = questionId,
            Text = "Correct",
            IsCorrect = true
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await ApiTestHelpers.ReadJsonAsync<ChoiceResponse>(response);
        result!.Text.Should().Be("Correct");
    }

    [Fact]
    public async Task CreateChoice_Should_Return_404_When_Question_Not_Found()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var response = await client.PostAsJsonAsync("/api/choices/", new ChoiceRequest
        {
            QuestionId = 9999,
            Text = "Ghost",
            IsCorrect = false
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetChoicesByQuestion_Should_Return_200()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);
        var questionId = await SeedQuestionAsync(factory);
        await SeedChoiceAsync(factory, questionId, "One");
        await SeedChoiceAsync(factory, questionId, "Two");

        var response = await client.GetAsync($"/api/choices/by-question/{questionId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ApiTestHelpers.ReadJsonAsync<IEnumerable<ChoiceResponse>>(response);
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetChoiceById_Should_Return_200_When_Found()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);
        var questionId = await SeedQuestionAsync(factory);
        var choiceId = await SeedChoiceAsync(factory, questionId, "Lookup");

        var response = await client.GetAsync($"/api/choices/{choiceId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateChoice_Should_Return_200()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);
        var questionId = await SeedQuestionAsync(factory);
        var choiceId = await SeedChoiceAsync(factory, questionId, "Old");

        var response = await client.PutAsJsonAsync($"/api/choices/{choiceId}", new ChoiceRequest
        {
            QuestionId = questionId,
            Text = "Updated",
            IsCorrect = false
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ApiTestHelpers.ReadJsonAsync<ChoiceResponse>(response);
        result!.Text.Should().Be("Updated");
    }

    [Fact]
    public async Task DeleteChoice_Should_Return_204()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);
        var questionId = await SeedQuestionAsync(factory);
        var choiceId = await SeedChoiceAsync(factory, questionId, "Delete me");

        var response = await client.DeleteAsync($"/api/choices/{choiceId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private static async Task<int> SeedQuestionAsync(ApiTestApplicationFactory factory)
    {
        var topic = new Topic { Title = $"Topic-{Guid.NewGuid()}" };
        var question = new Question
        {
            Topic = topic,
            Text = $"Question-{Guid.NewGuid()}"
        };

        await factory.SeedAsync(async context =>
        {
            context.Questions.Add(question);
            await Task.CompletedTask;
        });

        return question.Id;
    }

    private static async Task<int> SeedChoiceAsync(ApiTestApplicationFactory factory, int questionId, string text)
    {
        var choice = new Choice
        {
            QuestionId = questionId,
            Text = text,
            IsCorrect = text == "One"
        };

        await factory.SeedAsync(async context =>
        {
            context.Choices.Add(choice);
            await Task.CompletedTask;
        });

        return choice.Id;
    }
}
