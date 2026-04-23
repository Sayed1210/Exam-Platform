using Xunit;
using Moq;
using FluentAssertions;
using ExamApi.Repositories;  
using ExamApi.Services;                
using ExamApi.DTOs.Requests;

public class ExamServiceTests
{
	private readonly Mock<IExamRepository> _repoMock;
	private readonly ExamService _service;

	
	public ExamServiceTests()
	{
		_repoMock = new Mock<IExamRepository>();
		_service = new ExamService(_repoMock.Object);
	} 

	// ✅ Test 1 
	[Fact]
	public async Task CreateExamAsync_Should_Create_Exam()
	{
		var dto = new CreateExamDto
		{
			Title = "Algo",
			DurationMins = 50,
			StartTime = DateTime.Now,
			EndTime = DateTime.Now.AddMinutes(50)
		};

		_repoMock
			.Setup(r => r.AddAsync(It.IsAny<Exam>()))
			.Returns(Task.CompletedTask);

		var result = await _service.CreateExamAsync(dto);

		result.Title.Should().Be("Algo");
		_repoMock.Verify(r => r.AddAsync(It.IsAny<Exam>()), Times.Once);
	}

	// ✅ Test 2
	[Fact]
	public async Task GetAllExamsAsync_Should_Return_List()
	{
		var exams = new List<Exam>
		{
			new Exam { Title = "A", DurationMins = 30 },
			new Exam { Title = "B", DurationMins = 60 }
		};

		_repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(exams);

		var result = await _service.GetAllExamsAsync();

		result.Should().HaveCount(2);
	}

	// ✅ Test 3
	[Fact]
	public async Task GetExamByIdAsync_Should_Return_Exam_When_Found()
	{
		var exam = new Exam { Id = 1, Title = "Algo" };

		_repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(exam);

		var result = await _service.GetExamByIdAsync(1);

		result.Should().NotBeNull();
		result!.Title.Should().Be("Algo");
	}

	// ✅ Test 4
	[Fact]
	public async Task GetExamByIdAsync_Should_Return_Null_When_NotFound()
	{
		_repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Exam?)null);

		var result = await _service.GetExamByIdAsync(1);

		result.Should().BeNull();
	}

	// ✅ Test 5
	[Fact]
	public async Task UpdateExamAsync_Should_Update_Exam()
	{
		var exam = new Exam { Id = 1, Title = "Old" };

		_repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(exam);

		var dto = new CreateExamDto
		{
			Title = "New",
			DurationMins = 60,
			StartTime = DateTime.Now,
			EndTime = DateTime.Now.AddMinutes(60)
		};

		var result = await _service.UpdateExamAsync(1, dto);

		result.Should().NotBeNull();
		result!.Title.Should().Be("New");
		_repoMock.Verify(r => r.UpdateAsync(exam), Times.Once);
	}

	// ✅ Test 6
	[Fact]
	public async Task UpdateExamAsync_Should_Return_Null_When_NotFound()
	{
		_repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Exam?)null);

		var dto = new CreateExamDto();

		var result = await _service.UpdateExamAsync(1, dto);

		result.Should().BeNull();
	}

	// ✅ Test 7
	[Fact]
	public async Task DeleteExamAsync_Should_Delete_Exam()
	{
		var exam = new Exam { Id = 1 };

		_repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(exam);

		var result = await _service.DeleteExamAsync(1);

		result.Should().BeTrue();
		_repoMock.Verify(r => r.DeleteAsync(exam), Times.Once);
	}

	// ✅ Test 8
	[Fact]
	public async Task DeleteExamAsync_Should_Return_False_When_NotFound()
	{
		_repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Exam?)null);

		var result = await _service.DeleteExamAsync(1);

		result.Should().BeFalse();
	}

} 