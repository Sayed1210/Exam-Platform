using Exam.Data;
using Exam.Models;
using Exam.Repo;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Exam.IntegrationTests;

public class QuestionRepositoryTests
{
    [Fact]
    public async Task GetByIdAsync_Should_Return_Question_With_Topic_And_Choices()
    {
        var databaseName = Guid.NewGuid().ToString();
        await using var context = RepositoryTestContextFactory.Create(databaseName);
        var topic = new Topic { Title = "C#" };
        var question = new Question
        {
            Topic = topic,
            Text = "What does CLR stand for?",
            Choices =
            {
                new Choice { Text = "Common Language Runtime", IsCorrect = true },
                new Choice { Text = "Compiled Language Runner", IsCorrect = false }
            }
        };

        context.Questions.Add(question);
        await context.SaveChangesAsync();

        var repository = new QuestionRepository(context);

        var result = await repository.GetByIdAsync(question.Id);

        result.Should().NotBeNull();
        result!.Topic.Should().NotBeNull();
        result.Topic!.Title.Should().Be("C#");
        result.Choices.Should().HaveCount(2);
        result.Choices.Should().Contain(c => c.Text == "Common Language Runtime" && c.IsCorrect);
    }

    [Fact]
    public async Task GetByTopicIdAsync_Should_Return_Only_Matching_Questions_As_NoTracking()
    {
        var databaseName = Guid.NewGuid().ToString();
        await using (var arrangeContext = RepositoryTestContextFactory.Create(databaseName))
        {
            var includedTopic = new Topic { Title = "Databases" };
            var excludedTopic = new Topic { Title = "Networks" };

            arrangeContext.Questions.AddRange(
                new Question
                {
                    Topic = includedTopic,
                    Text = "What is a primary key?",
                    Choices =
                    {
                        new Choice { Text = "Unique row identifier", IsCorrect = true }
                    }
                },
                new Question
                {
                    Topic = excludedTopic,
                    Text = "What is DNS?",
                    Choices =
                    {
                        new Choice { Text = "Domain Name System", IsCorrect = true }
                    }
                });

            await arrangeContext.SaveChangesAsync();
        }

        await using var context = RepositoryTestContextFactory.Create(databaseName);
        var repository = new QuestionRepository(context);
        var includedTopicId = await context.Topics
            .Where(topic => topic.Title == "Databases")
            .Select(topic => topic.Id)
            .SingleAsync();

        var results = (await repository.GetByTopicIdAsync(includedTopicId)).ToList();

        results.Should().ContainSingle();
        results[0].TopicId.Should().Be(includedTopicId);
        results[0].Topic.Should().NotBeNull();
        results[0].Choices.Should().ContainSingle();

        context.ChangeTracker.Entries<Question>()
            .Should()
            .NotContain(entry => entry.Entity.Id == results[0].Id && entry.State != EntityState.Detached);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_All_Questions_With_Related_Data()
    {
        var databaseName = Guid.NewGuid().ToString();
        await using var context = RepositoryTestContextFactory.Create(databaseName);
        var topic = new Topic { Title = "Algorithms" };
        context.Questions.AddRange(
            new Question
            {
                Topic = topic,
                Text = "What is Big O?",
                Choices = { new Choice { Text = "Complexity notation", IsCorrect = true } }
            },
            new Question
            {
                Topic = topic,
                Text = "What is recursion?",
                Choices = { new Choice { Text = "A function calling itself", IsCorrect = true } }
            });

        await context.SaveChangesAsync();

        var repository = new QuestionRepository(context);

        var results = (await repository.GetAllAsync()).ToList();

        results.Should().HaveCount(2);
        results.Should().OnlyContain(question => question.Topic != null);
        results.Should().OnlyContain(question => question.Choices.Count == 1);
    }

    [Fact]
    public async Task CreateAsync_Should_Persist_Question()
    {
        var databaseName = Guid.NewGuid().ToString();
        await using var context = RepositoryTestContextFactory.Create(databaseName);
        var topic = new Topic { Title = "Testing" };
        context.Topics.Add(topic);
        await context.SaveChangesAsync();

        var repository = new QuestionRepository(context);
        var question = new Question
        {
            TopicId = topic.Id,
            Text = "Why write tests?"
        };

        var result = await repository.CreateAsync(question);

        result.Id.Should().BeGreaterThan(0);
        (await context.Questions.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task UpdateAsync_Should_Persist_Changes()
    {
        var databaseName = Guid.NewGuid().ToString();
        await using var context = RepositoryTestContextFactory.Create(databaseName);
        var topic = new Topic { Title = "Design" };
        var question = new Question { Topic = topic, Text = "Old text" };
        context.Questions.Add(question);
        await context.SaveChangesAsync();

        var repository = new QuestionRepository(context);
        question.Text = "New text";

        var result = await repository.UpdateAsync(question);

        result.Text.Should().Be("New text");
        (await context.Questions.SingleAsync()).Text.Should().Be("New text");
    }

    [Fact]
    public async Task DeleteAsync_Should_Remove_Question()
    {
        var databaseName = Guid.NewGuid().ToString();
        await using var context = RepositoryTestContextFactory.Create(databaseName);
        var topic = new Topic { Title = "Cleanup" };
        var question = new Question { Topic = topic, Text = "Delete me" };
        context.Questions.Add(question);
        await context.SaveChangesAsync();

        var repository = new QuestionRepository(context);

        await repository.DeleteAsync(question);

        (await context.Questions.AnyAsync()).Should().BeFalse();
    }

    [Fact]
    public async Task ExistsAsync_Should_Return_True_When_Question_Exists()
    {
        var databaseName = Guid.NewGuid().ToString();
        await using var context = RepositoryTestContextFactory.Create(databaseName);
        var topic = new Topic { Title = "Existence" };
        var question = new Question { Topic = topic, Text = "Present" };
        context.Questions.Add(question);
        await context.SaveChangesAsync();

        var repository = new QuestionRepository(context);

        var exists = await repository.ExistsAsync(question.Id);

        exists.Should().BeTrue();
    }
}
