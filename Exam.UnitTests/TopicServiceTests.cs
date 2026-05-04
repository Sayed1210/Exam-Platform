using Xunit;
using Moq;
using FluentAssertions;
using Exam.Models.dtos.requests;
using Exam.Repo;
using Exam.Service;
using Exam.Models;

public class TopicServiceTests
{
    private readonly Mock<ITopicRepository> _repoMock;
    private readonly TopicService _service;

    public TopicServiceTests()
    {
        _repoMock = new Mock<ITopicRepository>();
        _service = new TopicService(_repoMock.Object);
    }

    // ═══════════════════════════════════════════════════════════════
    // CREATE
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task CreateTopicAsync_Should_Create_Topic()
    {
        var dto = new TopicRequest { Title = "OOP" };

        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<Topic>()))
            .Returns(Task.CompletedTask);

        var result = await _service.CreateTopicAsync(dto);

        result.Should().NotBeNull();
        result.Title.Should().Be("OOP");

        _repoMock.Verify(r => r.AddAsync(It.IsAny<Topic>()), Times.Once);
    }

    // ═══════════════════════════════════════════════════════════════
    // GET ALL
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task GetAllTopicsAsync_Should_Return_All_Topics()
    {
        var topics = new List<Topic>
        {
            new Topic { Id = 1, Title = "Math" },
            new Topic { Id = 2, Title = "Science" }
        };

        _repoMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(topics);

        var result = await _service.GetAllTopicsAsync();

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllTopicsAsync_Should_Return_Empty()
    {
        _repoMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Topic>());

        var result = await _service.GetAllTopicsAsync();

        result.Should().BeEmpty();
    }

    // ═══════════════════════════════════════════════════════════════
    // GET BY ID
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task GetTopicByIdAsync_Should_Return_Topic()
    {
        var topic = new Topic { Id = 1, Title = "Math" };

        _repoMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(topic);

        var result = await _service.GetTopicByIdAsync(1);

        result.Should().NotBeNull();
        result!.Title.Should().Be("Math");
    }

    [Fact]
    public async Task GetTopicByIdAsync_Should_Return_Null()
    {
        _repoMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((Topic?)null);

        var result = await _service.GetTopicByIdAsync(1);

        result.Should().BeNull();
    }

    // ═══════════════════════════════════════════════════════════════
    // UPDATE
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task UpdateTopicAsync_Should_Update_Topic()
    {
        var topic = new Topic { Id = 1, Title = "Old" };
        var dto = new TopicRequest { Title = "New" };

        _repoMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(topic);

        _repoMock
            .Setup(r => r.UpdateAsync(topic))
            .Returns(Task.CompletedTask);

        var result = await _service.UpdateTopicAsync(1, dto);

        result.Should().NotBeNull();
        result!.Title.Should().Be("New");

        _repoMock.Verify(r => r.UpdateAsync(topic), Times.Once);
    }

    [Fact]
    public async Task UpdateTopicAsync_Should_Return_Null_When_NotFound()
    {
        _repoMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((Topic?)null);

        var dto = new TopicRequest { Title = "New" };

        var result = await _service.UpdateTopicAsync(1, dto);

        result.Should().BeNull();

        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Topic>()), Times.Never);
    }

    // ═══════════════════════════════════════════════════════════════
    // DELETE
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task DeleteTopicAsync_Should_Return_True()
    {
        var topic = new Topic { Id = 1 };

        _repoMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(topic);

        var result = await _service.DeleteTopicAsync(1);

        result.Should().BeTrue();

        _repoMock.Verify(r => r.DeleteAsync(topic), Times.Once);
    }

    [Fact]
    public async Task DeleteTopicAsync_Should_Return_False()
    {
        _repoMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((Topic?)null);

        var result = await _service.DeleteTopicAsync(1);

        result.Should().BeFalse();

        _repoMock.Verify(r => r.DeleteAsync(It.IsAny<Topic>()), Times.Never);
    }
}