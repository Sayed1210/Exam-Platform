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

   
    // Helpers - Reusable fake data
    

    private static Question MakeQuestion(int id = 1) => new()
    {
        Id = id,
        TopicId = 1,
        Text = $"Question {id}",
        ImageUrl = null,
        Topic = new Topic { Id = 1, Title = "Math" },
        Choices = new List<Choice>
            {
                new Choice { Id = 1, Text = "Choice A", IsCorrect = true,  ImageUrl = null },
                new Choice { Id = 2, Text = "Choice B", IsCorrect = false, ImageUrl = null }
            }
    };

    private static QuestionRequest MakeRequest() => new()
    {
        TopicId = 1,
        Text = "What is 2+2?",
        ImageUrl = null
    };

    // ───────────────────────────────────────────
    // GET ALL
    // ───────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_Should_Return_All_Questions()
    {
        // Arrange
        var questions = new List<Question>
            {
                MakeQuestion(1),
                MakeQuestion(2),
                MakeQuestion(3)
            };

        _repoMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(questions);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Select(q => q.Id).Should().Contain([1, 2, 3]);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_Empty_When_No_Questions()
    {
        // Arrange
        _repoMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Question>());

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    // 
    // GET BY TOPIC ID
    // 

    [Fact]
    public async Task GetByTopicIdAsync_Should_Return_Questions_For_Topic()
    {
        // Arrange
        var questions = new List<Question>
            {
                MakeQuestion(1),
                MakeQuestion(2)
            };

        _repoMock
            .Setup(r => r.GetByTopicIdAsync(1))
            .ReturnsAsync(questions);

        // Act
        var result = await _service.GetByTopicIdAsync(1);

        // Assert
        result.Should().HaveCount(2);
        result.All(q => q.TopicId == 1).Should().BeTrue();
    }

    [Fact]
    public async Task GetByTopicIdAsync_Should_Return_Empty_When_No_Questions_For_Topic()
    {
        // Arrange
        _repoMock
            .Setup(r => r.GetByTopicIdAsync(99))
            .ReturnsAsync(new List<Question>());

        // Act
        var result = await _service.GetByTopicIdAsync(99);

        // Assert
        result.Should().BeEmpty();
    }

    // 
    // GET BY ID
    // 

    [Fact]
    public async Task GetByIdAsync_Should_Return_Question_When_Found()
    {
        // Arrange
        var question = MakeQuestion(1);

        _repoMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(question);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Text.Should().Be("Question 1");
        result.TopicTitle.Should().Be("Math");
        result.Choices.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Not_Found()
    {
        // Arrange
        _repoMock
            .Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync((Question?)null);

        // Act
        var result = await _service.GetByIdAsync(99);

        // Assert
        result.Should().BeNull();
    }

    // 
    // CREATE
    // 

    [Fact]
    public async Task CreateAsync_Should_Create_And_Return_Question()
    {
        // Arrange
        var request = MakeRequest();
        var createdQuestion = MakeQuestion(1);

        _repoMock
            .Setup(r => r.CreateAsync(It.IsAny<Question>()))
            .ReturnsAsync(createdQuestion);

        // Reload after create
        _repoMock
            .Setup(r => r.GetByIdAsync(createdQuestion.Id))
            .ReturnsAsync(createdQuestion);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.TopicId.Should().Be(request.TopicId);
        result.Text.Should().Be(createdQuestion.Text);
        result.Choices.Should().HaveCount(2);

        _repoMock.Verify(r => r.CreateAsync(It.IsAny<Question>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Should_Map_Choices_Correctly()
    {
        // Arrange
        var request = MakeRequest();
        var createdQuestion = MakeQuestion(1);

        _repoMock
            .Setup(r => r.CreateAsync(It.IsAny<Question>()))
            .ReturnsAsync(createdQuestion);

        _repoMock
            .Setup(r => r.GetByIdAsync(createdQuestion.Id))
            .ReturnsAsync(createdQuestion);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.Choices.Should().HaveCount(2);
        result.Choices.First().Text.Should().Be("Choice A");
        result.Choices.First().IsCorrect.Should().BeTrue();
        result.Choices.Last().Text.Should().Be("Choice B");
        result.Choices.Last().IsCorrect.Should().BeFalse();
    }

    // 
    // UPDATE
    // 

    [Fact]
    public async Task UpdateAsync_Should_Update_And_Return_Question()
    {
        // Arrange
        var existing = MakeQuestion(1);
        var request = new QuestionRequest
        {
            TopicId = 2,
            Text = "Updated Text",
            ImageUrl = "http://image.com/new.png"
        };

        var updatedQuestion = new Question
        {
            Id = 1,
            TopicId = 2,
            Text = "Updated Text",
            ImageUrl = "http://image.com/new.png",
            Topic = new Topic { Id = 2, Title = "Science" },
            Choices = new List<Choice>()
        };

        _repoMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(existing);          // first call  → existing

        _repoMock
       .Setup(r => r.UpdateAsync(existing))
       .ReturnsAsync(existing);

        // Second GetByIdAsync call after update returns updated question
        _repoMock
            .SetupSequence(r => r.GetByIdAsync(1))
            .ReturnsAsync(existing)           // 1st call inside UpdateAsync
            .ReturnsAsync(updatedQuestion);   // 2nd call after UpdateAsync

        // Act
        var result = await _service.UpdateAsync(1, request);

        // Assert
        result.Should().NotBeNull();
        result!.Text.Should().Be("Updated Text");
        result.TopicId.Should().Be(2);

        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Question>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Should_Return_Null_When_Not_Found()
    {
        // Arrange
        _repoMock
            .Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync((Question?)null);

        var request = MakeRequest();

        // Act
        var result = await _service.UpdateAsync(99, request);

        // Assert
        result.Should().BeNull();

        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Question>()), Times.Never);
    }

    // 
    // DELETE
    // 

    [Fact]
    public async Task DeleteAsync_Should_Delete_And_Return_True()
    {
        // Arrange
        var question = MakeQuestion(1);

        _repoMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(question);

        _repoMock
            .Setup(r => r.DeleteAsync(question))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeleteAsync(1);

        // Assert
        result.Should().BeTrue();

        _repoMock.Verify(r => r.DeleteAsync(question), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Should_Return_False_When_Not_Found()
    {
        // Arrange
        _repoMock
            .Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync((Question?)null);

        // Act
        var result = await _service.DeleteAsync(99);

        // Assert
        result.Should().BeFalse();

        _repoMock.Verify(r => r.DeleteAsync(It.IsAny<Question>()), Times.Never);
    }
}