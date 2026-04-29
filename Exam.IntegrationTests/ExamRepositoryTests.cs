using Exam.Models;
using Exam.Repo;
using ExamEntity = Exam.Models.Exam;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Exam.IntegrationTests;

public class ExamRepositoryTests
{
    [Fact]
    public async Task AddAsync_Should_Persist_Exam()
    {
        var databaseName = Guid.NewGuid().ToString();
        await using var context = RepositoryTestContextFactory.Create(databaseName);
        var repository = new ExamRepository(context);

        await repository.AddAsync(new ExamEntity
        {
            Title = "Midterm",
            DurationMins = 60
        });

        (await context.Exams.SingleAsync()).Title.Should().Be("Midterm");
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_Exams_With_ExamQuestions()
    {
        var databaseName = Guid.NewGuid().ToString();
        await using var context = RepositoryTestContextFactory.Create(databaseName);
        var exam = await SeedExamGraphAsync(context);
        var repository = new ExamRepository(context);

        var results = (await repository.GetAllAsync()).ToList();

        results.Should().ContainSingle();
        results[0].Id.Should().Be(exam.Id);
        results[0].ExamQuestions.Should().ContainSingle();
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Exam_With_ExamQuestions()
    {
        var databaseName = Guid.NewGuid().ToString();
        await using var context = RepositoryTestContextFactory.Create(databaseName);
        var exam = await SeedExamGraphAsync(context);
        var repository = new ExamRepository(context);

        var result = await repository.GetByIdAsync(exam.Id);

        result.Should().NotBeNull();
        result!.ExamQuestions.Should().ContainSingle();
    }

    [Fact]
    public async Task GetWithQuestionsAndChoicesAsync_Should_Return_Full_Graph()
    {
        var databaseName = Guid.NewGuid().ToString();
        await using var context = RepositoryTestContextFactory.Create(databaseName);
        var exam = await SeedExamGraphAsync(context);
        var repository = new ExamRepository(context);

        var result = await repository.GetWithQuestionsAndChoicesAsync(exam.Id);

        result.Should().NotBeNull();
        result!.ExamQuestions.Should().ContainSingle();
        result.ExamQuestions.Single().Question.Should().NotBeNull();
        result.ExamQuestions.Single().Question!.Choices.Should().ContainSingle();
    }

    [Fact]
    public async Task UpdateAsync_Should_Persist_Changes()
    {
        var databaseName = Guid.NewGuid().ToString();
        await using var context = RepositoryTestContextFactory.Create(databaseName);
        var exam = new ExamEntity { Title = "Initial", DurationMins = 45 };
        context.Exams.Add(exam);
        await context.SaveChangesAsync();

        var repository = new ExamRepository(context);
        exam.Title = "Updated";

        await repository.UpdateAsync(exam);

        (await context.Exams.SingleAsync()).Title.Should().Be("Updated");
    }

    [Fact]
    public async Task DeleteAsync_Should_Remove_Exam()
    {
        var databaseName = Guid.NewGuid().ToString();
        await using var context = RepositoryTestContextFactory.Create(databaseName);
        var exam = new ExamEntity { Title = "Remove", DurationMins = 30 };
        context.Exams.Add(exam);
        await context.SaveChangesAsync();

        var repository = new ExamRepository(context);

        await repository.DeleteAsync(exam);

        (await context.Exams.AnyAsync()).Should().BeFalse();
    }

    [Fact]
    public async Task AssignQuestionsAsync_Should_Persist_Junction_Rows()
    {
        var databaseName = Guid.NewGuid().ToString();
        await using var context = RepositoryTestContextFactory.Create(databaseName);
        var exam = new ExamEntity { Title = "Assignment", DurationMins = 50 };
        var topic = new Topic { Title = "Math" };
        var question = new Question { Topic = topic, Text = "2 + 2?" };
        context.Exams.Add(exam);
        context.Questions.Add(question);
        await context.SaveChangesAsync();

        var repository = new ExamRepository(context);

        await repository.AssignQuestionsAsync(
            [new ExamQuestion { ExamId = exam.Id, QuestionId = question.Id }]);

        (await context.ExamQuestions.SingleAsync()).ExamId.Should().Be(exam.Id);
    }

    [Fact]
    public async Task RemoveQuestionAsync_Should_Delete_Matching_Junction_Row_Only()
    {
        var databaseName = Guid.NewGuid().ToString();
        await using var context = RepositoryTestContextFactory.Create(databaseName);
        var exam = new ExamEntity { Title = "Removal", DurationMins = 50 };
        var topic = new Topic { Title = "Science" };
        var keptQuestion = new Question { Topic = topic, Text = "Kept" };
        var removedQuestion = new Question { Topic = topic, Text = "Removed" };
        context.Exams.Add(exam);
        context.Questions.AddRange(keptQuestion, removedQuestion);
        await context.SaveChangesAsync();

        context.ExamQuestions.AddRange(
            new ExamQuestion { ExamId = exam.Id, QuestionId = keptQuestion.Id },
            new ExamQuestion { ExamId = exam.Id, QuestionId = removedQuestion.Id });
        await context.SaveChangesAsync();

        var repository = new ExamRepository(context);

        await repository.RemoveQuestionAsync(exam.Id, removedQuestion.Id);

        var junctionRows = await context.ExamQuestions.ToListAsync();
        junctionRows.Should().ContainSingle();
        junctionRows[0].QuestionId.Should().Be(keptQuestion.Id);
    }

    [Fact]
    public async Task ExistsAsync_Should_Return_True_When_Exam_Exists()
    {
        var databaseName = Guid.NewGuid().ToString();
        await using var context = RepositoryTestContextFactory.Create(databaseName);
        var exam = new ExamEntity { Title = "Exists", DurationMins = 90 };
        context.Exams.Add(exam);
        await context.SaveChangesAsync();

        var repository = new ExamRepository(context);

        var exists = await repository.ExistsAsync(exam.Id);

        exists.Should().BeTrue();
    }

    private static async Task<ExamEntity> SeedExamGraphAsync(Exam.Data.ApiContext context)
    {
        var topic = new Topic { Title = "General" };
        var question = new Question
        {
            Topic = topic,
            Text = "What is testing?",
            Choices = { new Choice { Text = "Verification", IsCorrect = true } }
        };
        var exam = new ExamEntity
        {
            Title = "Repository exam",
            DurationMins = 75,
            ExamQuestions = { new ExamQuestion { Question = question } }
        };

        context.Exams.Add(exam);
        await context.SaveChangesAsync();
        return exam;
    }
}
