using Xunit;
using Moq;
using FluentAssertions;
using Exam.Models;
using Exam.Repo;
using Exam.Service;

namespace Exam.Tests.Services
{
    public class TopicServiceTests
    {
        private readonly Mock<ITopicRepository> _repoMock;
        private readonly TopicService _service;

        public TopicServiceTests()
        {
            _repoMock = new Mock<ITopicRepository>();
            _service = new TopicService(_repoMock.Object);
        }

        // ── HELPERS ───────────────────────────────────────────────────
        private static Topic MakeTopic(int id = 1, string title = "Math") => new()
        {
            Id = id,
            Title = title,
            Questions = []
        };

        private static TopicRequest MakeRequest(string title = "Math") => new()
        {
            Title = title
        };

        // ── CREATE ────────────────────────────────────────────────────
        [Fact]
        public async Task CreateTopicAsync_Should_Create_Topic()
        {
            _repoMock.Setup(r => r.AddAsync(It.IsAny<Topic>()))
                     .Returns(Task.CompletedTask);

            var result = await _service.CreateTopicAsync(MakeRequest("Science"));

            result.Should().NotBeNull();
            result.Title.Should().Be("Science");
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Topic>()), Times.Once);
        }

        [Fact]
        public async Task CreateTopicAsync_Should_Trim_Title()
        {
            _repoMock.Setup(r => r.AddAsync(It.IsAny<Topic>()))
                     .Returns(Task.CompletedTask);

            var result = await _service.CreateTopicAsync(new TopicRequest { Title = "  Science  " });

            result.Title.Should().Be("Science");
        }

        // ── GET ALL ───────────────────────────────────────────────────
        [Fact]
        public async Task GetAllTopicsAsync_Should_Return_All_Topics()
        {
            _repoMock.Setup(r => r.GetAllAsync())
                     .ReturnsAsync([MakeTopic(1, "Math"), MakeTopic(2, "Science")]);

            var result = await _service.GetAllTopicsAsync();

            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllTopicsAsync_Should_Return_Empty_When_No_Topics()
        {
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

            var result = await _service.GetAllTopicsAsync();

            result.Should().BeEmpty();
        }

        // ── GET BY ID ─────────────────────────────────────────────────
        [Fact]
        public async Task GetTopicByIdAsync_Should_Return_Topic_When_Found()
        {
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeTopic(1, "Math"));

            var result = await _service.GetTopicByIdAsync(1);

            result.Should().NotBeNull();
            result!.Title.Should().Be("Math");
            result.Id.Should().Be(1);
        }

        [Fact]
        public async Task GetTopicByIdAsync_Should_Return_Null_When_NotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Topic?)null);

            var result = await _service.GetTopicByIdAsync(1);

            result.Should().BeNull();
        }

        // ── UPDATE ────────────────────────────────────────────────────
        [Fact]
        public async Task UpdateTopicAsync_Should_Update_Title()
        {
            var topic = MakeTopic(1, "Old");

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(topic);
            _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Topic>())).Returns(Task.CompletedTask);

            var result = await _service.UpdateTopicAsync(1, new TopicRequest { Title = "New" });

            result.Should().NotBeNull();
            result!.Title.Should().Be("New");
            _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Topic>()), Times.Once);
        }

        [Fact]
        public async Task UpdateTopicAsync_Should_Return_Null_When_NotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Topic?)null);

            var result = await _service.UpdateTopicAsync(1, MakeRequest());

            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateTopicAsync_Should_Trim_Title()
        {
            var topic = MakeTopic(1, "Old");

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(topic);
            _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Topic>())).Returns(Task.CompletedTask);

            var result = await _service.UpdateTopicAsync(1, new TopicRequest { Title = "  New  " });

            result!.Title.Should().Be("New");
        }

        // ── DELETE ────────────────────────────────────────────────────
        [Fact]
        public async Task DeleteTopicAsync_Should_Return_True_When_Deleted()
        {
            var topic = MakeTopic(1);

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(topic);
            _repoMock.Setup(r => r.DeleteAsync(topic)).Returns(Task.CompletedTask);

            var result = await _service.DeleteTopicAsync(1);

            result.Should().BeTrue();
            _repoMock.Verify(r => r.DeleteAsync(topic), Times.Once);
        }

        [Fact]
        public async Task DeleteTopicAsync_Should_Return_False_When_NotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Topic?)null);

            var result = await _service.DeleteTopicAsync(1);

            result.Should().BeFalse();
        }

        // ── MAPPER ────────────────────────────────────────────────────
        [Fact]
        public async Task GetAllTopicsAsync_Should_Map_QuestionsCount()
        {
            var topic = new Topic
            {
                Id = 1,
                Title = "Math",
                Questions = [new Question(), new Question(), new Question()]
            };

            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync([topic]);

            var result = await _service.GetAllTopicsAsync();

            result.First().QuestionsCount.Should().Be(3);
        }
    }
}