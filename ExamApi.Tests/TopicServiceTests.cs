using Xunit;
using Moq;
using FluentAssertions;
using ExamApi.DTOs.Requests;
using ExamApi.DTOs.Responses;
using ExamApi.Repositories;
using ExamApi.Services;

    public class TopicServiceTests
    {
        private readonly Mock<ITopicRepository> _repoMock;
        private readonly TopicService _service;

        public TopicServiceTests()
        {
            _repoMock = new Mock<ITopicRepository>();
            _service = new TopicService(_repoMock.Object);
        }

        // ───────────────────────────────────────────
        // CREATE
        // ───────────────────────────────────────────

        [Fact]
        public async Task CreateTopicAsync_Should_Create_Topic()
        {
            // Arrange
            var dto = new TopicRequest { Title = "OOP" };

            _repoMock
                .Setup(r => r.TitleExistsAsync(dto.Title))
                .ReturnsAsync(false);

            _repoMock
                .Setup(r => r.AddAsync(It.IsAny<Topic>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateTopicAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be("OOP");

            _repoMock.Verify(r => r.AddAsync(It.IsAny<Topic>()), Times.Once);
        }

        [Fact]
        public async Task CreateTopicAsync_Should_Throw_When_Title_Already_Exists()
        {
            // Arrange
            var dto = new TopicRequest { Title = "Mathematics" };

            _repoMock
                .Setup(r => r.TitleExistsAsync(dto.Title))
                .ReturnsAsync(true);

            // Act
            Func<Task> act = async () => await _service.CreateTopicAsync(dto);

            // Assert
            await act.Should()
                .ThrowAsync<InvalidOperationException>() // ✅ FIXED
                .WithMessage("*Mathematics*");

            _repoMock.Verify(r => r.AddAsync(It.IsAny<Topic>()), Times.Never);
        }

        // ───────────────────────────────────────────
        // READ - GET ALL
        // ───────────────────────────────────────────

        [Fact]
        public async Task GetAllTopicsAsync_Should_Return_All_Topics()
        {
            // Arrange
            var topics = new List<Topic>
            {
                new Topic { Id = 1, Title = "Math" },
                new Topic { Id = 2, Title = "Science" },
                new Topic { Id = 3, Title = "History" }
            };

            _repoMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(topics);

            // Act
            var result = await _service.GetAllTopicsAsync();

            // Assert
            result.Should().HaveCount(3);
            result.Select(t => t.Title).Should()
                .Contain(new[] { "Math", "Science", "History" });
        }

        [Fact]
        public async Task GetAllTopicsAsync_Should_Return_Empty_When_No_Topics()
        {
            // Arrange
            _repoMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Topic>());

            // Act
            var result = await _service.GetAllTopicsAsync();

            // Assert
            result.Should().BeEmpty();
        }

        // ───────────────────────────────────────────
        // READ - GET BY ID
        // ───────────────────────────────────────────

        [Fact]
        public async Task GetTopicByIdAsync_Should_Return_Topic_When_Found()
        {
            // Arrange
            var topic = new Topic { Id = 1, Title = "Math" };

            _repoMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(topic);

            // Act
            var result = await _service.GetTopicByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.Title.Should().Be("Math");
        }

        [Fact]
        public async Task GetTopicByIdAsync_Should_Return_Null_When_Not_Found()
        {
            // Arrange
            _repoMock
                .Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((Topic?)null);

            // Act
            var result = await _service.GetTopicByIdAsync(99);

            // Assert
            result.Should().BeNull();
        }

        // ───────────────────────────────────────────
        // UPDATE
        // ───────────────────────────────────────────

        [Fact]
        public async Task UpdateTopicAsync_Should_Update_Topic()
        {
            // Arrange
            var topic = new Topic { Id = 1, Title = "Old Title" };
            var dto = new TopicRequest { Title = "New Title" };

            _repoMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(topic);

            _repoMock
                .Setup(r => r.TitleExistsAsync(dto.Title, 1))
                .ReturnsAsync(false);

            _repoMock
                .Setup(r => r.UpdateAsync(topic))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.UpdateTopicAsync(1, dto);

            // Assert
            result.Should().NotBeNull();
            result!.Title.Should().Be("New Title");

            _repoMock.Verify(r => r.UpdateAsync(topic), Times.Once);
        }

        [Fact]
        public async Task UpdateTopicAsync_Should_Return_Null_When_Not_Found()
        {
            // Arrange
            _repoMock
                .Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((Topic?)null);

            var dto = new TopicRequest { Title = "New Title" };

            // Act
            var result = await _service.UpdateTopicAsync(99, dto);

            // Assert
            result.Should().BeNull();

            _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Topic>()), Times.Never);
        }

        [Fact]
        public async Task UpdateTopicAsync_Should_Throw_When_Title_Already_Exists()
        {
            // Arrange
            var topic = new Topic { Id = 1, Title = "Old Title" };
            var dto = new TopicRequest { Title = "Duplicate Title" };

            _repoMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(topic);

            _repoMock
                .Setup(r => r.TitleExistsAsync(dto.Title, 1))
                .ReturnsAsync(true);

            // Act
            Func<Task> act = async () => await _service.UpdateTopicAsync(1, dto);

            // Assert
            await act.Should()
                .ThrowAsync<InvalidOperationException>() // ✅ FIXED
                .WithMessage("*Duplicate Title*");

            _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Topic>()), Times.Never);
        }

        // ───────────────────────────────────────────
        // DELETE
        // ───────────────────────────────────────────

        [Fact]
        public async Task DeleteTopicAsync_Should_Delete_And_Return_True()
        {
            // Arrange
            var topic = new Topic { Id = 1, Title = "Math" };

            _repoMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(topic);

            _repoMock
                .Setup(r => r.DeleteAsync(topic))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteTopicAsync(1);

            // Assert
            result.Should().BeTrue();

            _repoMock.Verify(r => r.DeleteAsync(topic), Times.Once);
        }

        [Fact]
        public async Task DeleteTopicAsync_Should_Return_False_When_Not_Found()
        {
            // Arrange
            _repoMock
                .Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((Topic?)null);

            // Act
            var result = await _service.DeleteTopicAsync(99);

            // Assert
            result.Should().BeFalse();

            _repoMock.Verify(r => r.DeleteAsync(It.IsAny<Topic>()), Times.Never);
        }
    }