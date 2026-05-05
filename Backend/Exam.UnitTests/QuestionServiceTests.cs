using Xunit;
using Moq;
using FluentAssertions;
using Exam.Models.dtos.requests;
using Exam.Repo;
using Exam.Service;
using Exam.Models;

public class QuestionServiceTests
{
    private readonly Mock<IQuestionRepository> _repoMock;
    private readonly QuestionService _service;

    public QuestionServiceTests()
    {
        _repoMock = new Mock<IQuestionRepository>();
        _service = new QuestionService(_repoMock.Object);
    }

    // ═══════════════════════════════════════════════════════════════
    // HELPERS
    // ═══════════════════════════════════════════════════════════════

    private static Question MakeQuestion(int id = 1) => new()
    {
        Id = id,
        TopicId = 1,
        Text = $"Question {id}",
        Topic = new Topic { Id = 1, Title = "Math" },
        Choices =
        [
            new Choice { Id = 1, Text = "Choice A", IsCorrect = true },
            new Choice { Id = 2, Text = "Choice B", IsCorrect = false }
        ]
    };

    private static QuestionRequest MakeCreateRequest() => new()
    {
        TopicId = 1,
        Text = "What is 2+2?",
        Choices =
        [
            new ChoiceRequest { Text = "4", IsCorrect = true },
            new ChoiceRequest { Text = "5", IsCorrect = false }
        ]
    };

    private static UpdateQuestionRequest MakeUpdateRequest() => new()
    {
        Text = "Updated text?",
        Choices =
        [
            new ChoiceRequest { Text = "4", IsCorrect = true },
            new ChoiceRequest { Text = "5", IsCorrect = false }
        ]
    };

    // ═══════════════════════════════════════════════════════════════
    // GET ALL
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task GetAllAsync_Should_Return_All_Questions()
    {
        _repoMock.Setup(r => r.GetAllAsync())
                 .ReturnsAsync([MakeQuestion(1), MakeQuestion(2)]);

        var result = await _service.GetAllAsync();

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_Empty_When_No_Questions()
    {
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

        var result = await _service.GetAllAsync();

        result.Should().BeEmpty();
    }

    // ═══════════════════════════════════════════════════════════════
    // GET BY ID
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task GetByIdAsync_Should_Return_Question()
    {
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeQuestion(1));

        var result = await _service.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Choices.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_NotFound()
    {
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Question?)null);

        var result = await _service.GetByIdAsync(1);

        result.Should().BeNull();
    }

    // ═══════════════════════════════════════════════════════════════
    // GET BY TOPIC
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task GetByTopicIdAsync_Should_Return_Questions_For_Topic()
    {
        _repoMock.Setup(r => r.GetByTopicIdAsync(1))
                 .ReturnsAsync([MakeQuestion(1), MakeQuestion(2)]);

        var result = await _service.GetByTopicIdAsync(1);

        result.Should().HaveCount(2);
        result.All(q => q.TopicId == 1).Should().BeTrue();
    }

    // ═══════════════════════════════════════════════════════════════
    // CREATE
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task CreateAsync_Should_Create_Question_With_Choices()
    {
        var request = MakeCreateRequest();
        var created = MakeQuestion(1);

        _repoMock.Setup(r => r.CreateAsync(It.IsAny<Question>()))
                 .ReturnsAsync(created);

        _repoMock.Setup(r => r.GetByIdAsync(1))
                 .ReturnsAsync(created);

        var result = await _service.CreateAsync(request);

        result.Should().NotBeNull();
        result.Choices.Should().HaveCount(2);
        _repoMock.Verify(r => r.CreateAsync(It.IsAny<Question>()), Times.Once);
    }

    // ═══════════════════════════════════════════════════════════════
    // UPDATE
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task UpdateAsync_Should_Update_Text_Only()
    {
        var existing = MakeQuestion(1);

        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Question>(), false))
                 .ReturnsAsync(new Question
                 {
                     Id = 1,
                     TopicId = 1,
                     Text = "Updated text?",
                     Topic = new Topic { Id = 1, Title = "Math" },
                     Choices = existing.Choices  // choices unchanged
                 });

        // Only text — no choices
        var request = new UpdateQuestionRequest { Text = "Updated text?" };

        var result = await _service.UpdateAsync(1, request);

        result.Should().NotBeNull();
        result!.Text.Should().Be("Updated text?");
        result.Choices.Should().HaveCount(2);  // choices unchanged
        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Question>(), false), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Choices_Only()
    {
        var existing = MakeQuestion(1);

        var updatedQuestion = new Question
        {
            Id = 1,
            TopicId = 1,
            Text = existing.Text,
            Topic = new Topic { Id = 1, Title = "Math" },
            Choices =
            [
                new Choice { Id = 3, Text = "New A", IsCorrect = true },
                new Choice { Id = 4, Text = "New B", IsCorrect = false }
            ]
        };

        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Question>(), true))
                 .ReturnsAsync(updatedQuestion);

        // Only choices — no text
        var request = new UpdateQuestionRequest
        {
            Choices =
            [
                new ChoiceRequest { Text = "New A", IsCorrect = true },
                new ChoiceRequest { Text = "New B", IsCorrect = false }
            ]
        };

        var result = await _service.UpdateAsync(1, request);

        result.Should().NotBeNull();
        result!.Text.Should().Be(existing.Text);  // text unchanged
        result.Choices.Should().HaveCount(2);
        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Question>(), true), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Should_Return_Null_When_NotFound()
    {
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Question?)null);

        var result = await _service.UpdateAsync(1, MakeUpdateRequest());

        result.Should().BeNull();
    }

    // ═══════════════════════════════════════════════════════════════
    // UPDATE SPECIFIC CHOICE
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task UpdateChoiceAsync_Should_Update_Specific_Choice()
    {
        var question = MakeQuestion(1);

        var updatedQuestion = new Question
        {
            Id = 1,
            TopicId = 1,
            Text = question.Text,
            Topic = question.Topic,
            Choices =
            [
                new Choice { Id = 1, Text = "Updated A", IsCorrect = false },
                new Choice { Id = 2, Text = "Choice B",  IsCorrect = true }
            ]
        };

        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(question);
        _repoMock.Setup(r => r.UpdateChoiceAsync(It.IsAny<Choice>()))
                 .ReturnsAsync(updatedQuestion);

        var request = new ChoiceRequest { Text = "Updated A", IsCorrect = false };

        var result = await _service.UpdateChoiceAsync(1, 1, request);

        result.Should().NotBeNull();
        _repoMock.Verify(r => r.UpdateChoiceAsync(It.IsAny<Choice>()), Times.Once);
    }

    [Fact]
    public async Task UpdateChoiceAsync_Should_Return_Null_When_Question_NotFound()
    {
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Question?)null);

        var result = await _service.UpdateChoiceAsync(1, 1, new ChoiceRequest { Text = "X", IsCorrect = true });

        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateChoiceAsync_Should_Return_Null_When_Choice_NotFound()
    {
        var question = MakeQuestion(1);

        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(question);

        // choiceId 99 doesn't exist
        var result = await _service.UpdateChoiceAsync(1, 99, new ChoiceRequest { Text = "X", IsCorrect = true });

        result.Should().BeNull();
    }

    // ═══════════════════════════════════════════════════════════════
    // DELETE
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task DeleteAsync_Should_Return_True()
    {
        var question = MakeQuestion(1);

        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(question);
        _repoMock.Setup(r => r.DeleteAsync(question)).Returns(Task.CompletedTask);

        var result = await _service.DeleteAsync(1);

        result.Should().BeTrue();
        _repoMock.Verify(r => r.DeleteAsync(question), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Should_Return_False_When_NotFound()
    {
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Question?)null);

        var result = await _service.DeleteAsync(1);

        result.Should().BeFalse();
    }

    // ═══════════════════════════════════════════════════════════════
    // PAGINATION
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task GetPagedAsync_Should_Return_Correct_Page()
    {
        var questions = new List<Question> { MakeQuestion(1), MakeQuestion(2) };

        _repoMock.Setup(r => r.GetPagedAsync(1, 2))
                 .ReturnsAsync((questions, 10));  // 10 total, 2 returned

        var result = await _service.GetPagedAsync(1, 2);

        result.Page.Should().Be(1);
        result.PageSize.Should().Be(2);
        result.TotalCount.Should().Be(10);
        result.TotalPages.Should().Be(5);  // 10/2 = 5
        result.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetPagedAsync_Should_Calculate_TotalPages_Correctly()
    {
        _repoMock.Setup(r => r.GetPagedAsync(1, 3))
                 .ReturnsAsync((new List<Question>(), 10));  // 10/3 = 3.33 → 4 pages

        var result = await _service.GetPagedAsync(1, 3);

        result.TotalPages.Should().Be(4);  // Math.Ceiling(10/3) = 4
    }
}