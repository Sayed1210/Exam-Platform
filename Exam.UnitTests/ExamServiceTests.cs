using Xunit;
using Moq;
using FluentAssertions;
using Exam.Models.dtos.requests;
using Exam.Repo;
using Exam.Service;

using ExamEntity = Exam.Models.Exam;
using ExamQuestionEntity = Exam.Models.ExamQuestion;

namespace Exam.UnitTests
{
    public class ExamServiceTests
    {
        private readonly Mock<IExamRepository> _repoMock;
        private readonly Mock<IQuestionRepository> _questionRepoMock;
        private readonly ExamService _service;

        public ExamServiceTests()
        {
            _repoMock = new Mock<IExamRepository>();
            _questionRepoMock = new Mock<IQuestionRepository>();
            _service = new ExamService(_repoMock.Object, _questionRepoMock.Object);
        }

        // ════════════════════════════════════════════════════════════════════
        // CREATE
        // ════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task CreateExamAsync_Should_Create_Exam()
        {
            var dto = new CreateExamDto
            {
                Title = "Algo",
                DurationMins = 50
            };

            _repoMock
                .Setup(r => r.AddAsync(It.IsAny<ExamEntity>()))
                .Returns(Task.CompletedTask);

            var result = await _service.CreateExamAsync(dto);

            result.Title.Should().Be("Algo");
            result.DurationMins.Should().Be(50);
            _repoMock.Verify(r => r.AddAsync(It.IsAny<ExamEntity>()), Times.Once);
        }

        [Fact]
        public async Task CreateExamAsync_Should_Throw_When_Title_Is_Empty()
        {
            var dto = new CreateExamDto
            {
                Title = "   ",
                DurationMins = 50
            };

            var act = async () => await _service.CreateExamAsync(dto);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*title*");
        }

        [Fact]
        public async Task CreateExamAsync_Should_Throw_When_Duration_Is_Zero()
        {
            var dto = new CreateExamDto
            {
                Title = "Algo",
                DurationMins = 0
            };

            var act = async () => await _service.CreateExamAsync(dto);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Duration*");
        }

        [Fact]
        public async Task CreateExamAsync_Should_Throw_When_Dto_Is_Null()
        {
            var act = async () => await _service.CreateExamAsync(null!);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        // ════════════════════════════════════════════════════════════════════
        // GET ALL
        // ════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task GetAllExamsAsync_Should_Return_List()
        {
            var exams = new List<ExamEntity>
            {
                new ExamEntity { Title = "A", DurationMins = 30 },
                new ExamEntity { Title = "B", DurationMins = 60 }
            };

            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(exams);

            var result = await _service.GetAllExamsAsync();

            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllExamsAsync_Should_Return_Empty_When_No_Exams()
        {
            _repoMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<ExamEntity>());

            var result = await _service.GetAllExamsAsync();

            result.Should().BeEmpty();
        }

        // ════════════════════════════════════════════════════════════════════
        // GET BY ID
        // ════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task GetExamByIdAsync_Should_Return_Exam_When_Found()
        {
            var exam = new ExamEntity { Id = 1, Title = "Algo" };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(exam);

            var result = await _service.GetExamByIdAsync(1);

            result.Should().NotBeNull();
            result!.Title.Should().Be("Algo");
        }

        [Fact]
        public async Task GetExamByIdAsync_Should_Return_Null_When_NotFound()
        {
            _repoMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync((ExamEntity?)null);

            var result = await _service.GetExamByIdAsync(1);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetExamByIdAsync_Should_Throw_When_Id_Is_Zero()
        {
            var act = async () => await _service.GetExamByIdAsync(0);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*positive*");
        }

        [Fact]
        public async Task GetExamByIdAsync_Should_Throw_When_Id_Is_Negative()
        {
            var act = async () => await _service.GetExamByIdAsync(-5);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*positive*");
        }

        // ════════════════════════════════════════════════════════════════════
        // UPDATE
        // ════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task UpdateExamAsync_Should_Update_Exam()
        {
            var exam = new ExamEntity { Id = 1, Title = "Old" };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(exam);

            var dto = new CreateExamDto
            {
                Title = "New",
                DurationMins = 60
            };

            var result = await _service.UpdateExamAsync(1, dto);

            result.Should().NotBeNull();
            result!.Title.Should().Be("New");
            _repoMock.Verify(r => r.UpdateAsync(exam), Times.Once);
        }

        [Fact]
        public async Task UpdateExamAsync_Should_Return_Null_When_NotFound()
        {
            _repoMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync((ExamEntity?)null);

            var dto = new CreateExamDto
            {
                Title = "New",
                DurationMins = 60
            };

            var result = await _service.UpdateExamAsync(1, dto);

            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateExamAsync_Should_Throw_When_Title_Is_Empty()
        {
            var dto = new CreateExamDto
            {
                Title = "",
                DurationMins = 60
            };

            var act = async () => await _service.UpdateExamAsync(1, dto);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*title*");
        }

        [Fact]
        public async Task UpdateExamAsync_Should_Throw_When_Id_Is_Zero()
        {
            var dto = new CreateExamDto
            {
                Title = "New",
                DurationMins = 60
            };

            var act = async () => await _service.UpdateExamAsync(0, dto);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*positive*");
        }

        // ════════════════════════════════════════════════════════════════════
        // DELETE
        // ════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task DeleteExamAsync_Should_Delete_Exam()
        {
            var exam = new ExamEntity { Id = 1 };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(exam);

            var result = await _service.DeleteExamAsync(1);

            result.Should().BeTrue();
            _repoMock.Verify(r => r.DeleteAsync(exam), Times.Once);
        }

        [Fact]
        public async Task DeleteExamAsync_Should_Return_False_When_NotFound()
        {
            _repoMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync((ExamEntity?)null);

            var result = await _service.DeleteExamAsync(1);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteExamAsync_Should_Throw_When_Id_Is_Zero()
        {
            var act = async () => await _service.DeleteExamAsync(0);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*positive*");
        }

        // ════════════════════════════════════════════════════════════════════
        // ASSIGN QUESTIONS
        // ════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task AssignQuestionsAsync_Should_Assign_Questions()
        {
            var exam = new ExamEntity
            {
                Id = 1,
                Title = "Algo",
                ExamQuestions = new List<ExamQuestionEntity>()
            };

            _repoMock
                .SetupSequence(r => r.GetByIdAsync(1))
                .ReturnsAsync(exam)   // 1st call - fetch exam
                .ReturnsAsync(exam);  // 2nd call - reload after assign

            _questionRepoMock
                .Setup(r => r.ExistsAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _repoMock
                .Setup(r => r.AssignQuestionsAsync(It.IsAny<List<ExamQuestionEntity>>()))
                .Returns(Task.CompletedTask);

            var result = await _service.AssignQuestionsAsync(1, new List<int> { 2, 3 });

            result.Should().NotBeNull();
            _repoMock.Verify(
                r => r.AssignQuestionsAsync(It.IsAny<List<ExamQuestionEntity>>()),
                Times.Once);
        }

        [Fact]
        public async Task AssignQuestionsAsync_Should_Throw_When_Exam_NotFound()
        {
            _repoMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync((ExamEntity?)null);

            var act = async () =>
                await _service.AssignQuestionsAsync(1, new List<int> { 2 });

            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("*Exam*");
        }

        [Fact]
        public async Task AssignQuestionsAsync_Should_Throw_When_QuestionIds_Empty()
        {
            var act = async () =>
                await _service.AssignQuestionsAsync(1, new List<int>());

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*empty*");
        }

        [Fact]
        public async Task AssignQuestionsAsync_Should_Throw_When_Duplicate_Ids_In_Request()
        {
            var act = async () =>
                await _service.AssignQuestionsAsync(1, new List<int> { 2, 2 });

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Duplicate*");
        }

        [Fact]
        public async Task AssignQuestionsAsync_Should_Throw_When_Question_NotFound()
        {
            var exam = new ExamEntity
            {
                Id = 1,
                ExamQuestions = new List<ExamQuestionEntity>()
            };

            _repoMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(exam);

            _questionRepoMock
                .Setup(r => r.ExistsAsync(99))
                .ReturnsAsync(false);

            var act = async () =>
                await _service.AssignQuestionsAsync(1, new List<int> { 99 });

            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("*Questions not found*");
        }

        [Fact]
        public async Task AssignQuestionsAsync_Should_Throw_When_All_Already_Assigned()
        {
            var exam = new ExamEntity
            {
                Id = 1,
                ExamQuestions = new List<ExamQuestionEntity>
                {
                    new ExamQuestionEntity { ExamId = 1, QuestionId = 2 }
                }
            };

            _repoMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(exam);

            _questionRepoMock
                .Setup(r => r.ExistsAsync(2))
                .ReturnsAsync(true);

            var act = async () =>
                await _service.AssignQuestionsAsync(1, new List<int> { 2 });

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*already assigned*");
        }

        // ════════════════════════════════════════════════════════════════════
        // REMOVE QUESTION
        // ════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task RemoveQuestionAsync_Should_Remove_Question()
        {
            var exam = new ExamEntity
            {
                Id = 1,
                ExamQuestions = new List<ExamQuestionEntity>
                {
                    new ExamQuestionEntity { ExamId = 1, QuestionId = 2 }
                }
            };

            _repoMock
                .SetupSequence(r => r.GetByIdAsync(1))
                .ReturnsAsync(exam)   // 1st call - fetch exam
                .ReturnsAsync(exam);  // 2nd call - reload after remove

            _repoMock
                .Setup(r => r.RemoveQuestionAsync(1, 2))
                .Returns(Task.CompletedTask);

            var result = await _service.RemoveQuestionAsync(1, 2);

            result.Should().NotBeNull();
            _repoMock.Verify(r => r.RemoveQuestionAsync(1, 2), Times.Once);
        }

        [Fact]
        public async Task RemoveQuestionAsync_Should_Throw_When_Exam_NotFound()
        {
            _repoMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync((ExamEntity?)null);

            var act = async () => await _service.RemoveQuestionAsync(1, 2);

            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("*Exam*");
        }

        [Fact]
        public async Task RemoveQuestionAsync_Should_Throw_When_Question_Not_Assigned()
        {
            var exam = new ExamEntity
            {
                Id = 1,
                ExamQuestions = new List<ExamQuestionEntity>()
            };

            _repoMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(exam);

            var act = async () => await _service.RemoveQuestionAsync(1, 99);

            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("*not assigned*");
        }

        [Fact]
        public async Task RemoveQuestionAsync_Should_Throw_When_ExamId_Is_Zero()
        {
            var act = async () => await _service.RemoveQuestionAsync(0, 2);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*positive*");
        }
    }
}