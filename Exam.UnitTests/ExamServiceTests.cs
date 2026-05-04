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
        // HELPERS
        // ════════════════════════════════════════════════════════════════════

        private static ExamEntity MakeExamWithQuestions(int id = 1) => new()
        {
            Id = id,
            Title = $"Exam {id}",
            DurationMins = 60,
            ExamQuestions =
            [
                new() { ExamId = id, QuestionId = 1, Question = new() { Id = 1, Text = "Q1", Choices = [] } },
                new() { ExamId = id, QuestionId = 2, Question = new() { Id = 2, Text = "Q2", Choices = [] } }
            ]
        };

        private static ExamEntity MakeEmptyExam(int id = 1) => new()
        {
            Id = id,
            Title = $"Exam {id}",
            DurationMins = 60,
            ExamQuestions = []
        };

        // ════════════════════════════════════════════════════════════════════
        // CREATE
        // ════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task CreateExamAsync_Should_Create_Exam()
        {
            var dto = new CreateExamRequest
            {
                Title = "Algo",
                DurationMins = 50,
                QuestionIds = []
            };

            _repoMock
                .Setup(r => r.AddAsync(It.IsAny<ExamEntity>()))
                .Returns(Task.CompletedTask);

            _repoMock
                .Setup(r => r.GetWithQuestionsAndChoicesAsync(It.IsAny<int>()))
                .ReturnsAsync(new ExamEntity
                {
                    Id = 1,
                    Title = "Algo",
                    DurationMins = 50,
                    ExamQuestions = []
                });

            var result = await _service.CreateExamAsync(dto);

            result.Title.Should().Be("Algo");
            result.DurationMins.Should().Be(50);
            _repoMock.Verify(r => r.AddAsync(It.IsAny<ExamEntity>()), Times.Once);
        }

        [Fact]
        public async Task CreateExamAsync_Should_Create_Exam_With_Questions()
        {
            var dto = new CreateExamRequest
            {
                Title = "Algo",
                DurationMins = 50,
                QuestionIds = [1, 2]
            };

            _repoMock
                .Setup(r => r.AddAsync(It.IsAny<ExamEntity>()))
                .Returns(Task.CompletedTask);

            _repoMock
                .Setup(r => r.GetWithQuestionsAndChoicesAsync(It.IsAny<int>()))
                .ReturnsAsync(MakeExamWithQuestions());

            var result = await _service.CreateExamAsync(dto);

            result.TotalQuestions.Should().Be(2);
            _repoMock.Verify(r => r.AddAsync(It.IsAny<ExamEntity>()), Times.Once);
        }

        // ════════════════════════════════════════════════════════════════════
        // GET ALL
        // ════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task GetAllExamsAsync_Should_Return_List()
        {
            _repoMock.Setup(r => r.GetAllAsync())
                     .ReturnsAsync([MakeEmptyExam(1), MakeEmptyExam(2)]);

            var result = await _service.GetAllExamsAsync();

            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllExamsAsync_Should_Return_Empty_When_No_Exams()
        {
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

            var result = await _service.GetAllExamsAsync();

            result.Should().BeEmpty();
        }

        // ════════════════════════════════════════════════════════════════════
        // GET BY ID
        // ════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task GetExamByIdAsync_Should_Return_Exam_When_Found()
        {
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeEmptyExam(1));

            var result = await _service.GetExamByIdAsync(1);

            result.Should().NotBeNull();
            result!.Title.Should().Be("Exam 1");
        }

        [Fact]
        public async Task GetExamByIdAsync_Should_Return_Null_When_NotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((ExamEntity?)null);

            var result = await _service.GetExamByIdAsync(1);

            result.Should().BeNull();
        }

        // ════════════════════════════════════════════════════════════════════
        // GET WITH QUESTIONS
        // ════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task GetExamWithQuestionsAsync_Should_Return_Exam_With_Questions()
        {
            _repoMock.Setup(r => r.GetWithQuestionsAndChoicesAsync(1))
                     .ReturnsAsync(MakeExamWithQuestions(1));

            var result = await _service.GetExamWithQuestionsAsync(1);

            result.Should().NotBeNull();
            result!.TotalQuestions.Should().Be(2);
            result.Questions.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetExamWithQuestionsAsync_Should_Return_Null_When_NotFound()
        {
            _repoMock.Setup(r => r.GetWithQuestionsAndChoicesAsync(1))
                     .ReturnsAsync((ExamEntity?)null);

            var result = await _service.GetExamWithQuestionsAsync(1);

            result.Should().BeNull();
        }

        // ════════════════════════════════════════════════════════════════════
        // UPDATE
        // ════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task UpdateExamAsync_Should_Update_Title_Only()
        {
            var exam = MakeEmptyExam(1);

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(exam);
            _repoMock.Setup(r => r.UpdateAsync(It.IsAny<ExamEntity>(), It.IsAny<List<int>?>()))
                     .Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.GetWithQuestionsAndChoicesAsync(1))
                     .ReturnsAsync(new ExamEntity
                     {
                         Id = 1,
                         Title = "New Title",
                         DurationMins = 60,
                         ExamQuestions = []
                     });

            var result = await _service.UpdateExamAsync(1, new UpdateExamRequest { Title = "New Title" });

            result.Should().NotBeNull();
            result!.Title.Should().Be("New Title");
            result.DurationMins.Should().Be(60);
        }

        [Fact]
        public async Task UpdateExamAsync_Should_Update_Duration_Only()
        {
            var exam = MakeEmptyExam(1);

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(exam);
            _repoMock.Setup(r => r.UpdateAsync(It.IsAny<ExamEntity>(), It.IsAny<List<int>?>()))
                     .Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.GetWithQuestionsAndChoicesAsync(1))
                     .ReturnsAsync(new ExamEntity
                     {
                         Id = 1,
                         Title = "Exam 1",
                         DurationMins = 90,
                         ExamQuestions = []
                     });

            var result = await _service.UpdateExamAsync(1, new UpdateExamRequest { DurationMins = 90 });

            result.Should().NotBeNull();
            result!.DurationMins.Should().Be(90);
            result.Title.Should().Be("Exam 1");
        }

        [Fact]
        public async Task UpdateExamAsync_Should_Update_Questions()
        {
            var exam = MakeEmptyExam(1);

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(exam);
            _repoMock.Setup(r => r.UpdateAsync(It.IsAny<ExamEntity>(), It.IsAny<List<int>?>()))
                     .Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.GetWithQuestionsAndChoicesAsync(1))
                     .ReturnsAsync(MakeExamWithQuestions(1));

            var result = await _service.UpdateExamAsync(1, new UpdateExamRequest { QuestionIds = [1, 2] });

            result.Should().NotBeNull();
            result!.TotalQuestions.Should().Be(2);
        }

        [Fact]
        public async Task UpdateExamAsync_Should_Return_Null_When_NotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((ExamEntity?)null);

            var result = await _service.UpdateExamAsync(1, new UpdateExamRequest { Title = "New" });

            result.Should().BeNull();
        }

        // ════════════════════════════════════════════════════════════════════
        // DELETE
        // ════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task DeleteExamAsync_Should_Delete_Exam()
        {
            var exam = MakeEmptyExam(1);

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(exam);
            _repoMock.Setup(r => r.DeleteAsync(exam)).Returns(Task.CompletedTask);

            var result = await _service.DeleteExamAsync(1);

            result.Should().BeTrue();
            _repoMock.Verify(r => r.DeleteAsync(exam), Times.Once);
        }

        [Fact]
        public async Task DeleteExamAsync_Should_Return_False_When_NotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((ExamEntity?)null);

            var result = await _service.DeleteExamAsync(1);

            result.Should().BeFalse();
        }

        // ════════════════════════════════════════════════════════════════════
        // ASSIGN QUESTIONS
        // ════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task AssignQuestionsAsync_Should_Assign_New_Questions()
        {
            var exam = MakeEmptyExam(1);

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(exam);
            _repoMock.Setup(r => r.AssignQuestionsAsync(It.IsAny<List<ExamQuestionEntity>>()))
                     .Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.GetWithQuestionsAndChoicesAsync(1))
                     .ReturnsAsync(new ExamEntity
                     {
                         Id = 1,
                         ExamQuestions =
                         [
                             new() { ExamId = 1, QuestionId = 2, Question = new() { Id = 2, Text = "Q2", Choices = [] } },
                             new() { ExamId = 1, QuestionId = 3, Question = new() { Id = 3, Text = "Q3", Choices = [] } }
                         ]
                     });

            var result = await _service.AssignQuestionsAsync(1, [2, 3]);

            result.Should().NotBeNull();
            result.TotalQuestions.Should().Be(2);
            _repoMock.Verify(
                r => r.AssignQuestionsAsync(It.IsAny<List<ExamQuestionEntity>>()),
                Times.Once);
        }

        [Fact]
        public async Task AssignQuestionsAsync_Should_Skip_Already_Assigned_Questions()
        {
            var exam = new ExamEntity
            {
                Id = 1,
                ExamQuestions =
                [
                    new() { ExamId = 1, QuestionId = 2 }  // already assigned
                ]
            };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(exam);
            _repoMock.Setup(r => r.AssignQuestionsAsync(It.IsAny<List<ExamQuestionEntity>>()))
                     .Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.GetWithQuestionsAndChoicesAsync(1))
                     .ReturnsAsync(new ExamEntity
                     {
                         Id = 1,
                         ExamQuestions =
                         [
                             new() { ExamId = 1, QuestionId = 2, Question = new() { Id = 2, Text = "Q2", Choices = [] } },
                             new() { ExamId = 1, QuestionId = 3, Question = new() { Id = 3, Text = "Q3", Choices = [] } }
                         ]
                     });

            var result = await _service.AssignQuestionsAsync(1, [2, 3]);

            result.TotalQuestions.Should().Be(2);
            _repoMock.Verify(
                r => r.AssignQuestionsAsync(It.Is<List<ExamQuestionEntity>>(
                    list => list.Count == 1 && list[0].QuestionId == 3)),
                Times.Once);
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
                ExamQuestions =
                [
                    new() { ExamId = 1, QuestionId = 2 }
                ]
            };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(exam);
            _repoMock.Setup(r => r.RemoveQuestionAsync(1, 2)).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.GetWithQuestionsAndChoicesAsync(1))
                     .ReturnsAsync(new ExamEntity
                     {
                         Id = 1,
                         ExamQuestions = []
                     });

            var result = await _service.RemoveQuestionAsync(1, 2);

            result.Should().NotBeNull();
            result.TotalQuestions.Should().Be(0);
            _repoMock.Verify(r => r.RemoveQuestionAsync(1, 2), Times.Once);
        }

        [Fact]
        public async Task RemoveQuestionAsync_Should_Remove_Even_When_Not_Assigned()
        {
            var exam = new ExamEntity
            {
                Id = 1,
                ExamQuestions = []
            };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(exam);
            _repoMock.Setup(r => r.RemoveQuestionAsync(1, 99)).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.GetWithQuestionsAndChoicesAsync(1))
                     .ReturnsAsync(new ExamEntity
                     {
                         Id = 1,
                         ExamQuestions = []
                     });

            var result = await _service.RemoveQuestionAsync(1, 99);

            result.Should().NotBeNull();
        }
    }
}