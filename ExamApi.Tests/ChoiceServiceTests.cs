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
        private readonly Mock<IQuestionRepository> _questionRepoMock;
        private readonly ChoiceService _service;

        public ChoiceServiceTests()
        {
            _repoMock = new Mock<IChoiceRepository>();
            _questionRepoMock = new Mock<IQuestionRepository>();
            _service = new ChoiceService(_repoMock.Object, _questionRepoMock.Object);
        }

        // ───────────────────────────────────────────
        // Helpers
        // ───────────────────────────────────────────

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

        // ───────────────────────────────────────────
        // GET ALL BY QUESTION ID
        // ───────────────────────────────────────────

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

            // ✅ Mock question exists
            _questionRepoMock
                .Setup(r => r.ExistsAsync(1))
                .ReturnsAsync(true);

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
            // ✅ Mock question exists
            _questionRepoMock
                .Setup(r => r.ExistsAsync(1))
                .ReturnsAsync(true);

            _repoMock
                .Setup(r => r.GetAllByQuestionIdAsync(1))
                .ReturnsAsync(new List<Choice>());

            // Act
            var result = await _service.GetAllByQuestionIdAsync(1);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllByQuestionIdAsync_Should_Throw_When_Question_Not_Found()
        {
            // Arrange
            // ✅ Mock question does NOT exist
            _questionRepoMock
                .Setup(r => r.ExistsAsync(99))
                .ReturnsAsync(false);

            // Act
            Func<Task> act = async () => await _service.GetAllByQuestionIdAsync(99);

            // Assert
            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("*99*");
        }

        // ───────────────────────────────────────────
        // GET BY ID
        // ───────────────────────────────────────────

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

        [Fact]
        public async Task GetByIdAsync_Should_Throw_When_Id_Is_Invalid()
        {
            // Act
            Func<Task> act = async () => await _service.GetByIdAsync(0);

            // Assert
            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("*positive*");
        }

        // ───────────────────────────────────────────
        // CREATE
        // ───────────────────────────────────────────

        [Fact]
        public async Task CreateAsync_Should_Create_And_Return_Choice()
        {
            // Arrange
            var request = MakeRequest();
            var createdChoice = MakeChoice(1);

            // ✅ Mock question exists
            _questionRepoMock
                .Setup(r => r.ExistsAsync(request.QuestionId))
                .ReturnsAsync(true);

            // ✅ Mock no existing choices (for max 6 and duplicate checks)
            _repoMock
                .Setup(r => r.GetAllByQuestionIdAsync(request.QuestionId))
                .ReturnsAsync(new List<Choice>());

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

            // ✅ Mock question 5 exists
            _questionRepoMock
                .Setup(r => r.ExistsAsync(5))
                .ReturnsAsync(true);

            // ✅ Mock no existing choices
            _repoMock
                .Setup(r => r.GetAllByQuestionIdAsync(5))
                .ReturnsAsync(new List<Choice>());

            _repoMock
                .Setup(r => r.CreateAsync(It.IsAny<Choice>()))
                .ReturnsAsync((Choice c) => c);

            // Act
            var result = await _service.CreateAsync(request);

            // Assert
            result.QuestionId.Should().Be(5);
            result.Text.Should().Be("My Answer");
            result.IsCorrect.Should().BeFalse();
            result.ImageUrl.Should().Be("http://image.com/img.png");
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_When_Question_Not_Found()
        {
            // Arrange
            var request = MakeRequest();

            _questionRepoMock
                .Setup(r => r.ExistsAsync(request.QuestionId))
                .ReturnsAsync(false);

            // Act
            Func<Task> act = async () => await _service.CreateAsync(request);

            // Assert
            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage($"*{request.QuestionId}*");
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_When_Text_Is_Empty()
        {
            // Arrange
            var request = new ChoiceRequest
            {
                QuestionId = 1,
                Text = "",
                IsCorrect = false
            };

            _questionRepoMock
                .Setup(r => r.ExistsAsync(1))
                .ReturnsAsync(true);

            // Act
            Func<Task> act = async () => await _service.CreateAsync(request);

            // Assert
            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("*empty*");
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_When_Max_Choices_Reached()
        {
            // Arrange
            var request = new ChoiceRequest
            {
                QuestionId = 1,
                Text = "Choice 7",
                IsCorrect = false
            };

            _questionRepoMock
                .Setup(r => r.ExistsAsync(1))
                .ReturnsAsync(true);

            // ✅ Mock 6 existing choices (max reached)
            var existingChoices = Enumerable.Range(1, 6)
                .Select(i => new Choice { Id = i, QuestionId = 1, Text = $"Choice {i}" })
                .ToList();

            _repoMock
                .Setup(r => r.GetAllByQuestionIdAsync(1))
                .ReturnsAsync(existingChoices);

            // Act
            Func<Task> act = async () => await _service.CreateAsync(request);

            // Assert
            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("*6*");
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_When_Duplicate_Text()
        {
            // Arrange
            var request = new ChoiceRequest
            {
                QuestionId = 1,
                Text = "Choice 1", // already exists
                IsCorrect = false
            };

            _questionRepoMock
                .Setup(r => r.ExistsAsync(1))
                .ReturnsAsync(true);

            // ✅ Mock existing choice with same text
            _repoMock
                .Setup(r => r.GetAllByQuestionIdAsync(1))
                .ReturnsAsync(new List<Choice>
                {
                    new Choice { Id = 1, QuestionId = 1, Text = "Choice 1" }
                });

            // Act
            Func<Task> act = async () => await _service.CreateAsync(request);

            // Assert
            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("*Choice 1*");
        }

        // ───────────────────────────────────────────
        // UPDATE
        // ───────────────────────────────────────────

        [Fact]
        public async Task UpdateAsync_Should_Update_And_Return_Choice()
        {
            // Arrange
            var existing = MakeChoice(1);

            var request = new ChoiceRequest
            {
                QuestionId = 1,
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

            // ✅ Mock no duplicate siblings
            _repoMock
                .Setup(r => r.GetAllByQuestionIdAsync(1))
                .ReturnsAsync(new List<Choice> { existing });

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
                QuestionId = 99,
                Text = "Updated Text",
                IsCorrect = false,
                ImageUrl = null
            };

            _repoMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(existing);

            _repoMock
                .Setup(r => r.GetAllByQuestionIdAsync(1))
                .ReturnsAsync(new List<Choice> { existing });

            _repoMock
                .Setup(r => r.UpdateAsync(existing))
                .ReturnsAsync(existing);

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

        [Fact]
        public async Task UpdateAsync_Should_Throw_When_Id_Is_Invalid()
        {
            // Act
            Func<Task> act = async () => await _service.UpdateAsync(0, MakeRequest());

            // Assert
            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("*positive*");
        }

        [Fact]
        public async Task UpdateAsync_Should_Throw_When_Duplicate_Text()
        {
            // Arrange
            var existing = MakeChoice(1);

            var request = new ChoiceRequest
            {
                QuestionId = 1,
                Text = "Choice 2", // duplicate of sibling
                IsCorrect = false
            };

            _repoMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(existing);

            // ✅ Sibling with same text exists
            _repoMock
                .Setup(r => r.GetAllByQuestionIdAsync(1))
                .ReturnsAsync(new List<Choice>
                {
                    existing,
                    new Choice { Id = 2, QuestionId = 1, Text = "Choice 2" }
                });

            // Act
            Func<Task> act = async () => await _service.UpdateAsync(1, request);

            // Assert
            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("*Choice 2*");
        }

        // ───────────────────────────────────────────
        // DELETE
        // ───────────────────────────────────────────

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

        [Fact]
        public async Task DeleteAsync_Should_Throw_When_Id_Is_Invalid()
        {
            // Act
            Func<Task> act = async () => await _service.DeleteAsync(0);

            // Assert
            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("*positive*");
        }
    }
