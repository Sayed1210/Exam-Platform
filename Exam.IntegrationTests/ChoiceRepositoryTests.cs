using Exam.Models;
using Exam.Repo;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Exam.IntegrationTests;

public class ChoiceRepositoryTests
{
    [Fact]
    public async Task GetAllByQuestionIdAsync_Should_Return_Only_Matching_Choices_As_NoTracking()
    {
        var databaseName = Guid.NewGuid().ToString();
        await using (var arrangeContext = RepositoryTestContextFactory.Create(databaseName))
        {
            var topic = new Topic { Title = "C#" };
            arrangeContext.Questions.AddRange(
                new Question
                {
                    Topic = topic,
                    Text = "Question 1",
                    Choices =
                    {
                        new Choice { Text = "A", IsCorrect = true },
                        new Choice { Text = "B", IsCorrect = false }
                    }
                },
                new Question
                {
                    Topic = topic,
                    Text = "Question 2",
                    Choices =
                    {
                        new Choice { Text = "C", IsCorrect = true }
                    }
                });

            await arrangeContext.SaveChangesAsync();
        }

        await using var context = RepositoryTestContextFactory.Create(databaseName);
        var repository = new ChoiceRepository(context);
        var questionId = await context.Questions
            .Where(question => question.Text == "Question 1")
            .Select(question => question.Id)
            .SingleAsync();

        var results = (await repository.GetAllByQuestionIdAsync(questionId)).ToList();

        results.Should().HaveCount(2);
        results.Should().OnlyContain(choice => choice.QuestionId == questionId);
        context.ChangeTracker.Entries<Choice>()
            .Should()
            .NotContain(entry => entry.Entity.Id == results[0].Id && entry.State != EntityState.Detached);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Choice_When_Found()
    {
        var databaseName = Guid.NewGuid().ToString();
        await using var context = RepositoryTestContextFactory.Create(databaseName);
        var choice = await SeedChoiceAsync(context);
        var repository = new ChoiceRepository(context);

        var result = await repository.GetByIdAsync(choice.Id);

        result.Should().NotBeNull();
        result!.Text.Should().Be("Correct answer");
    }

    [Fact]
    public async Task CreateAsync_Should_Persist_Choice()
    {
        var databaseName = Guid.NewGuid().ToString();
        await using var context = RepositoryTestContextFactory.Create(databaseName);
        var question = await SeedQuestionAsync(context);
        var repository = new ChoiceRepository(context);

        var result = await repository.CreateAsync(new Choice
        {
            QuestionId = question.Id,
            Text = "New option",
            IsCorrect = false
        });

        result.Id.Should().BeGreaterThan(0);
        (await context.Choices.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task UpdateAsync_Should_Persist_Changes()
    {
        var databaseName = Guid.NewGuid().ToString();
        await using var context = RepositoryTestContextFactory.Create(databaseName);
        var choice = await SeedChoiceAsync(context);
        var repository = new ChoiceRepository(context);
        choice.Text = "Updated answer";

        var result = await repository.UpdateAsync(choice);

        result.Text.Should().Be("Updated answer");
        (await context.Choices.SingleAsync()).Text.Should().Be("Updated answer");
    }

    [Fact]
    public async Task DeleteAsync_Should_Remove_Choice()
    {
        var databaseName = Guid.NewGuid().ToString();
        await using var context = RepositoryTestContextFactory.Create(databaseName);
        var choice = await SeedChoiceAsync(context);
        var repository = new ChoiceRepository(context);

        await repository.DeleteAsync(choice);

        (await context.Choices.AnyAsync()).Should().BeFalse();
    }

    [Fact]
    public async Task ExistsAsync_Should_Return_True_When_Choice_Exists()
    {
        var databaseName = Guid.NewGuid().ToString();
        await using var context = RepositoryTestContextFactory.Create(databaseName);
        var choice = await SeedChoiceAsync(context);
        var repository = new ChoiceRepository(context);

        var exists = await repository.ExistsAsync(choice.Id);

        exists.Should().BeTrue();
    }

    private static async Task<Question> SeedQuestionAsync(Exam.Data.ApiContext context)
    {
        var topic = new Topic { Title = "Basics" };
        var question = new Question { Topic = topic, Text = "What is .NET?" };
        context.Questions.Add(question);
        await context.SaveChangesAsync();
        return question;
    }

    private static async Task<Choice> SeedChoiceAsync(Exam.Data.ApiContext context)
    {
        var question = await SeedQuestionAsync(context);
        var choice = new Choice
        {
            QuestionId = question.Id,
            Text = "Correct answer",
            IsCorrect = true
        };

        context.Choices.Add(choice);
        await context.SaveChangesAsync();
        return choice;
    }
}
