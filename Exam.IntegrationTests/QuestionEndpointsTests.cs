using System.Net;
using System.Net.Http.Json;
using Exam.Models;
using Exam.Models.dtos.requests;
using Exam.Models.dtos.responses;
using FluentAssertions;
using Xunit;

namespace Exam.IntegrationTests;

public class QuestionEndpointsTests
{
    [Fact]
    public async Task CreateQuestion_Should_Return_201()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);
        var topicId = await SeedTopicAsync(factory);

        var response = await client.PostAsJsonAsync("/api/questions/", new QuestionRequest
        {
            TopicId = topicId,
            Text = "What is polymorphism?"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await ApiTestHelpers.ReadJsonAsync<QuestionResponse>(response);
        result!.Text.Should().Be("What is polymorphism?");
    }

    [Fact]
    public async Task CreateQuestion_Should_Return_400_When_TopicId_Is_Invalid()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var response = await client.PostAsJsonAsync("/api/questions/", new QuestionRequest
        {
            TopicId = 0,
            Text = "Invalid"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetQuestionById_Should_Return_200_When_Found()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);
        var questionId = await SeedQuestionAsync(factory, "What is OOP?");

        var response = await client.GetAsync($"/api/questions/{questionId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetQuestionsByTopic_Should_Return_200()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);
        var topicId = await SeedTopicAsync(factory);
        await SeedQuestionAsync(factory, "Question 1", topicId);
        await SeedQuestionAsync(factory, "Question 2", topicId);

        var response = await client.GetAsync($"/api/questions/by-topic/{topicId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ApiTestHelpers.ReadJsonAsync<IEnumerable<QuestionResponse>>(response);
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateQuestion_Should_Return_200()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);
        var topicId = await SeedTopicAsync(factory);
        var questionId = await SeedQuestionAsync(factory, "Old question", topicId);

        var response = await client.PutAsJsonAsync($"/api/questions/{questionId}", new QuestionRequest
        {
            TopicId = topicId,
            Text = "Updated question"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ApiTestHelpers.ReadJsonAsync<QuestionResponse>(response);
        result!.Text.Should().Be("Updated question");
    }

    [Fact]
    public async Task DeleteQuestion_Should_Return_204()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);
        var questionId = await SeedQuestionAsync(factory, "Delete me");

        var response = await client.DeleteAsync($"/api/questions/{questionId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private static async Task<int> SeedTopicAsync(ApiTestApplicationFactory factory)
    {
        var topic = new Topic { Title = $"Topic-{Guid.NewGuid()}" };
        await factory.SeedAsync(async context =>
        {
            context.Topics.Add(topic);
            await Task.CompletedTask;
        });
        return topic.Id;
    }

    private static async Task<int> SeedQuestionAsync(ApiTestApplicationFactory factory, string text, int? topicId = null)
    {
        if (!topicId.HasValue)
        {
            topicId = await SeedTopicAsync(factory);
        }

        var question = new Question
        {
            TopicId = topicId.Value,
            Text = text
        };

        await factory.SeedAsync(async context =>
        {
            context.Questions.Add(question);
            await Task.CompletedTask;
        });

        return question.Id;
    }
}
