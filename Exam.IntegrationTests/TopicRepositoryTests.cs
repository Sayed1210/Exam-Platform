using Exam.Models;
using Exam.Repo;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Exam.IntegrationTests;

public class TopicRepositoryTests
{
    [Fact]
    public async Task GetAllAsync_Should_Return_All_Topics_With_Questions_As_NoTracking()
    {
        var databaseName = Guid.NewGuid().ToString();
        await using (var arrangeContext = RepositoryTestContextFactory.Create(databaseName))
        {
            arrangeContext.Topics.AddRange(
                new Topic
                {
                    Title = "Backend",
                    Questions = { new Question { Text = "What is an API?" } }
                },
                new Topic
                {
                    Title = "Frontend",
                    Questions = { new Question { Text = "What is the DOM?" } }
                });

            await arrangeContext.SaveChangesAsync();
        }

        await using var context = RepositoryTestContextFactory.Create(databaseName);
        var repository = new TopicRepository(context);

        var results = (await repository.GetAllAsync()).ToList();

        results.Should().HaveCount(2);
        results.Should().OnlyContain(topic => topic.Questions.Count == 1);
        context.ChangeTracker.Entries<Topic>()
            .Should()
            .BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Topic_With_Questions()
    {
        var databaseName = Guid.NewGuid().ToString();
        await using var context = RepositoryTestContextFactory.Create(databaseName);
        var topic = new Topic
        {
            Title = "Databases",
            Questions = { new Question { Text = "What is SQL?" } }
        };
        context.Topics.Add(topic);
        await context.SaveChangesAsync();

        var repository = new TopicRepository(context);

        var result = await repository.GetByIdAsync(topic.Id);

        result.Should().NotBeNull();
        result!.Questions.Should().ContainSingle();
    }

    [Fact]
    public async Task AddAsync_Should_Persist_Topic()
    {
        var databaseName = Guid.NewGuid().ToString();
        await using var context = RepositoryTestContextFactory.Create(databaseName);
        var repository = new TopicRepository(context);

        await repository.AddAsync(new Topic { Title = "New topic" });

        (await context.Topics.SingleAsync()).Title.Should().Be("New topic");
    }

    [Fact]
    public async Task UpdateAsync_Should_Persist_Changes()
    {
        var databaseName = Guid.NewGuid().ToString();
        await using var context = RepositoryTestContextFactory.Create(databaseName);
        var topic = new Topic { Title = "Old title" };
        context.Topics.Add(topic);
        await context.SaveChangesAsync();

        var repository = new TopicRepository(context);
        topic.Title = "Updated title";

        await repository.UpdateAsync(topic);

        (await context.Topics.SingleAsync()).Title.Should().Be("Updated title");
    }

    [Fact]
    public async Task DeleteAsync_Should_Remove_Topic()
    {
        var databaseName = Guid.NewGuid().ToString();
        await using var context = RepositoryTestContextFactory.Create(databaseName);
        var topic = new Topic { Title = "Delete me" };
        context.Topics.Add(topic);
        await context.SaveChangesAsync();

        var repository = new TopicRepository(context);

        await repository.DeleteAsync(topic);

        (await context.Topics.AnyAsync()).Should().BeFalse();
    }

    [Fact]
    public async Task ExistsAsync_Should_Return_True_When_Topic_Exists()
    {
        var databaseName = Guid.NewGuid().ToString();
        await using var context = RepositoryTestContextFactory.Create(databaseName);
        var topic = new Topic { Title = "Present" };
        context.Topics.Add(topic);
        await context.SaveChangesAsync();

        var repository = new TopicRepository(context);

        var exists = await repository.ExistsAsync(topic.Id);

        exists.Should().BeTrue();
    }

    [Fact]
    public async Task TitleExistsAsync_Should_Be_Case_Insensitive_And_Respect_ExcludeId()
    {
        var databaseName = Guid.NewGuid().ToString();
        await using var context = RepositoryTestContextFactory.Create(databaseName);
        var topic = new Topic { Title = "DotNet" };
        context.Topics.Add(topic);
        await context.SaveChangesAsync();

        var repository = new TopicRepository(context);

        (await repository.TitleExistsAsync("dotnet")).Should().BeTrue();
        (await repository.TitleExistsAsync("dotnet", topic.Id)).Should().BeFalse();
    }
}
