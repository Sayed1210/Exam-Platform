using Moq;
using Xunit;
using Xunit.Abstractions;
using Exam.Services;
using Exam.Repositories;
using Exam.Models;

public class CandidateServiceTests
{
    private readonly Mock<ICandidateRepository> _repoMock;
    private readonly CandidateService _service;
    private readonly ITestOutputHelper _output;

    public CandidateServiceTests(ITestOutputHelper output)
    {
        _repoMock = new Mock<ICandidateRepository>();
        _service = new CandidateService(_repoMock.Object);
        _output = output;
    }

    // test GetAllCandidates
    [Fact]
    public async Task GetAllCandidates_ShouldReturnListOfCandidates()
    {
        // Arrange
        var candidates = new List<Candidate>
        {
            new Candidate { Id = 1, Email = "a@test.com", FirstName = "A", LastName = "B", Phone = "123" }
        };

        _repoMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(candidates);

        // Act
        var result = await _service.GetAllCandidates();

        // Assert
        Assert.Single(result);
        Assert.Equal("a@test.com", result[0].Email);

        _repoMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    // test GetCandidateById (success case)
    [Fact]
    public async Task GetCandidateById_ShouldReturnCandidate_WhenExists()
    {
        // Arrange
        var candidate = new Candidate
        {
            Id = 1,
            Email = "test@test.com",
            FirstName = "John",
            LastName = "Doe",
            Phone = "111"
        };

        _repoMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(candidate);

        // Act
        var result = await _service.GetCandidateById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result!.Id);
    }

    // test GetCandidateById (not found)
    [Fact]
    public async Task GetCandidateById_ShouldThrowException_WhenNotFound()
    {
        // Arrange
        _repoMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync((Candidate?)null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GetCandidateById(1));
    }

    // test AddCandidate (success)
    [Fact]
    public async Task AddCandidate_ShouldAddCandidate_WhenNotExists()
    {
        // Arrange
        var dto = new CreateCandidate
        {
            Email = "new@test.com",
            FirstName = "A",
            LastName = "B",
            Phone = "123"
        };

        _repoMock.Setup(r => r.GetByEmailAsync(dto.Email))
                .ReturnsAsync((Candidate?)null);

        _repoMock.Setup(r => r.AddAsync(It.IsAny<Candidate>()))
                .Returns(Task.CompletedTask);

        // Act
        await _service.AddCandidate(dto);

        // Assert
        _repoMock.Verify(r => r.AddAsync(It.Is<Candidate>(
            c => c.Email == dto.Email
        )), Times.Once);
    }

    // test AddCandidate (duplicate email)
    [Fact]
    public async Task AddCandidate_ShouldThrow_WhenEmailExists()
    {
        // Arrange
        var dto = new CreateCandidate
        {
            Email = "exist@test.com"
        };

        _repoMock.Setup(r => r.GetByEmailAsync(dto.Email))
                .ReturnsAsync(new Candidate());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.AddCandidate(dto));
    }

    // test DeleteCandidate (success)
    [Fact]
    public async Task DeleteCandidate_ShouldDelete_WhenExists()
    {
        // Arrange
        var candidate = new Candidate { Id = 1 };

        _repoMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(candidate);

        _repoMock.Setup(r => r.DeleteAsync(1))
                .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteCandidate(1);

        // Assert
        _repoMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    // test DeleteCandidate (not found)
    [Fact]
    public async Task DeleteCandidate_ShouldThrow_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync((Candidate?)null);

        await Assert.ThrowsAsync<Exception>(() => _service.DeleteCandidate(1));
    }
}