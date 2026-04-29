using System.Net;
using System.Net.Http.Json;
using Exam.Models;
using Exam.Models.dtos.requests;
using Exam.Models.dtos.responses;
using FluentAssertions;
using Xunit;

namespace Exam.IntegrationTests;

public class TopicEndpointsTests
{
    [Fact]
    public async Task CreateTopic_Should_Return_201()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        var response = await client.PostAsJsonAsync("/api/topics/", new TopicRequest { Title = "Backend" });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await ApiTestHelpers.ReadJsonAsync<TopicResponse>(response);
        result!.Title.Should().Be("Backend");
    }

    [Fact]
    public async Task CreateTopic_Should_Return_400_For_Duplicate_Title()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);

        await factory.SeedAsync(async context =>
        {
            context.Topics.Add(new Topic { Title = "Backend" });
            await Task.CompletedTask;
        });

        var response = await client.PostAsJsonAsync("/api/topics/", new TopicRequest { Title = "backend" });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetTopicById_Should_Return_200_When_Found()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);
        var topicId = await SeedTopicAsync(factory, "Frontend");

        var response = await client.GetAsync($"/api/topics/{topicId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateTopic_Should_Return_200()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);
        var topicId = await SeedTopicAsync(factory, "Old");

        var response = await client.PutAsJsonAsync($"/api/topics/{topicId}", new TopicRequest { Title = "New" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ApiTestHelpers.ReadJsonAsync<TopicResponse>(response);
        result!.Title.Should().Be("New");
    }

    [Fact]
    public async Task DeleteTopic_Should_Return_204_When_Topic_Has_No_Questions()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);
        var topicId = await SeedTopicAsync(factory, "Disposable");

        var response = await client.DeleteAsync($"/api/topics/{topicId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteTopic_Should_Return_400_When_Topic_Has_Questions()
    {
        await using var factory = new ApiTestApplicationFactory();
        using var client = ApiTestHelpers.CreateClient(factory);
        var topicId = await SeedTopicWithQuestionAsync(factory, "Busy");

        var response = await client.DeleteAsync($"/api/topics/{topicId}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private static async Task<int> SeedTopicAsync(ApiTestApplicationFactory factory, string title)
    {
        var topic = new Topic { Title = title };
        await factory.SeedAsync(async context =>
        {
            context.Topics.Add(topic);
            await Task.CompletedTask;
        });
        return topic.Id;
    }

    private static async Task<int> SeedTopicWithQuestionAsync(ApiTestApplicationFactory factory, string title)
    {
        var topic = new Topic
        {
            Title = title,
            Questions = { new Question { Text = "Attached question" } }
        };

        await factory.SeedAsync(async context =>
        {
            context.Topics.Add(topic);
            await Task.CompletedTask;
        });

        return topic.Id;
    }
}
