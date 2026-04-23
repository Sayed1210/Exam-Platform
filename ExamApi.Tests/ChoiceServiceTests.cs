using Xunit;
using Moq;
using FluentAssertions;
using ExamApi.DTOs.Requests;
using ExamApi.DTOs.Responses;
using ExamApi.Repositories;
using ExamApi.Services;
public class ChoiceServiceTests
{
    private readonly Mock<IChoiceRepository> _repoMock;
    private readonly ChoiceService _service;

    public ChoiceServiceTests()
    {
        _repoMock = new Mock<IChoiceRepository>();
        _service = new ChoiceService(_repoMock.Object);
    }

    
    // Helpers - Reusable fake data
    

    private static Choice MakeChoice(int id = 1) => new()
    {
        Id = id,
        QuestionId = 1,
        Text = $"Choice {id}",
        IsCorrect = id == 1,
        ImageUrl = null
    };

    private static ChoiceRequest MakeRequest() => new()
    {
        QuestionId = 1,
        Text = "Choice A",
        IsCorrect = true,
        ImageUrl = null
    };

    // 
    // GET ALL BY QUESTION ID
    // 

    [Fact]
    public async Task GetAllByQuestionIdAsync_Should_Return_Choices_For_Question()
    {
        // Arrange
        var choices = new List<Choice>
            {
                MakeChoice(1),
                MakeChoice(2),
                MakeChoice(3)
            };

        _repoMock
            .Setup(r => r.GetAllByQuestionIdAsync(1))
            .ReturnsAsync(choices);

        // Act
        var result = await _service.GetAllByQuestionIdAsync(1);

        // Assert
        result.Should().HaveCount(3);
        result.All(c => c.QuestionId == 1).Should().BeTrue();
    }

    [Fact]
    public async Task GetAllByQuestionIdAsync_Should_Return_Empty_When_No_Choices()
    {
        // Arrange
        _repoMock
            .Setup(r => r.GetAllByQuestionIdAsync(99))
            .ReturnsAsync(new List<Choice>());

        // Act
        var result = await _service.GetAllByQuestionIdAsync(99);

        // Assert
        result.Should().BeEmpty();
    }

    // 
    // GET BY ID
  

    [Fact]
    public async Task GetByIdAsync_Should_Return_Choice_When_Found()
    {
        // Arrange
        var choice = MakeChoice(1);

        _repoMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(choice);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Text.Should().Be("Choice 1");
        result.IsCorrect.Should().BeTrue();
        result.QuestionId.Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Not_Found()
    {
        // Arrange
        _repoMock
            .Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync((Choice?)null);

        // Act
        var result = await _service.GetByIdAsync(99);

        // Assert
        result.Should().BeNull();
    }

    // 
    // CREATE
    // 

    [Fact]
    public async Task CreateAsync_Should_Create_And_Return_Choice()
    {
        // Arrange
        var request = MakeRequest();
        var createdChoice = MakeChoice(1);

        _repoMock
            .Setup(r => r.CreateAsync(It.IsAny<Choice>()))
            .ReturnsAsync(createdChoice);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.QuestionId.Should().Be(request.QuestionId);
        result.Text.Should().Be(createdChoice.Text);
        result.IsCorrect.Should().Be(createdChoice.IsCorrect);

        _repoMock.Verify(r => r.CreateAsync(It.IsAny<Choice>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Should_Map_Request_To_Choice_Correctly()
    {
        // Arrange
        var request = new ChoiceRequest
        {
            QuestionId = 5,
            Text = "My Answer",
            IsCorrect = false,
            ImageUrl = "http://image.com/img.png"
        };

        _repoMock
            .Setup(r => r.CreateAsync(It.IsAny<Choice>()))
            .ReturnsAsync((Choice c) => c); // return what was passed in

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.QuestionId.Should().Be(5);
        result.Text.Should().Be("My Answer");
        result.IsCorrect.Should().BeFalse();
        result.ImageUrl.Should().Be("http://image.com/img.png");
    }

    
    // UPDATE
    

    [Fact]
    public async Task UpdateAsync_Should_Update_And_Return_Choice()
    {
        // Arrange
        var existing = MakeChoice(1);

        var request = new ChoiceRequest
        {
            QuestionId = 1,       // should NOT update QuestionId
            Text = "Updated Text",
            IsCorrect = false,
            ImageUrl = "http://image.com/new.png"
        };

        var updatedChoice = new Choice
        {
            Id = 1,
            QuestionId = 1,
            Text = "Updated Text",
            IsCorrect = false,
            ImageUrl = "http://image.com/new.png"
        };

        _repoMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(existing);

        _repoMock
            .Setup(r => r.UpdateAsync(existing))
            .ReturnsAsync(updatedChoice);

        // Act
        var result = await _service.UpdateAsync(1, request);

        // Assert
        result.Should().NotBeNull();
        result!.Text.Should().Be("Updated Text");
        result.IsCorrect.Should().BeFalse();
        result.ImageUrl.Should().Be("http://image.com/new.png");

        _repoMock.Verify(r => r.UpdateAsync(existing), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Should_Not_Change_QuestionId()
    {
        // Arrange
        var existing = MakeChoice(1); // QuestionId = 1

        var request = new ChoiceRequest
        {
            QuestionId = 99, // attempt to change QuestionId
            Text = "Updated Text",
            IsCorrect = false,
            ImageUrl = null
        };

        _repoMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(existing);

        _repoMock
            .Setup(r => r.UpdateAsync(existing))
            .ReturnsAsync(existing); // QuestionId stays 1

        // Act
        var result = await _service.UpdateAsync(1, request);

        // Assert
        result!.QuestionId.Should().Be(1); // ✅ QuestionId NOT changed to 99
    }

    [Fact]
    public async Task UpdateAsync_Should_Return_Null_When_Not_Found()
    {
        // Arrange
        _repoMock
            .Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync((Choice?)null);

        var request = MakeRequest();

        // Act
        var result = await _service.UpdateAsync(99, request);

        // Assert
        result.Should().BeNull();

        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Choice>()), Times.Never);
    }

    // 
    // DELETE
    // 

    [Fact]
    public async Task DeleteAsync_Should_Delete_And_Return_True()
    {
        // Arrange
        var choice = MakeChoice(1);

        _repoMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(choice);

        _repoMock
            .Setup(r => r.DeleteAsync(choice))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeleteAsync(1);

        // Assert
        result.Should().BeTrue();

        _repoMock.Verify(r => r.DeleteAsync(choice), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Should_Return_False_When_Not_Found()
    {
        // Arrange
        _repoMock
            .Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync((Choice?)null);

        // Act
        var result = await _service.DeleteAsync(99);

        // Assert
        result.Should().BeFalse();

        _repoMock.Verify(r => r.DeleteAsync(It.IsAny<Choice>()), Times.Never);
    }
}