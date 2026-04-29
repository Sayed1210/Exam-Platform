using Xunit;
using Moq;
using Exam.Services;
using Exam.Repositories;
using Exam.Models;

namespace Exam.Tests;

public class VerifyInvitationServiceTests
{
    private readonly Mock<ICandidateExamRepository> _repoMock;
    private readonly VerifyInvitationService _service;

    public VerifyInvitationServiceTests()
    {
        _repoMock = new Mock<ICandidateExamRepository>();
        _service = new VerifyInvitationService(_repoMock.Object);
    }

    [Fact]
    public async Task VerifyInvitation_ReturnsNull_WhenTokenDoesNotExist()
    {
        // Arrange
        _repoMock.Setup(r => r.GetByInvitationTokenAsync("token123"))
            .ReturnsAsync((CandidateExam?)null);

        // Act
        var result = await _service.VerifyInvitation("token123");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task VerifyInvitation_ReturnsNull_WhenInvitationExpired()
    {
        // Arrange
        var candidateExam = new CandidateExam
        {
            InvitationToken = "token123",
            CandidateId = 1,
            ExamId = 10,
            ExpiryDate = DateTime.UtcNow.AddMinutes(-5), // expired
            Candidate = new Candidate
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com"
            }
        };

        _repoMock.Setup(r => r.GetByInvitationTokenAsync("token123"))
            .ReturnsAsync(candidateExam);

        // Act
        var result = await _service.VerifyInvitation("token123");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task VerifyInvitation_ReturnsResponse_WhenInvitationValid()
    {
        // Arrange
        var candidateExam = new CandidateExam
        {
            InvitationToken = "token123",
            CandidateId = 1,
            ExamId = 10,
            ExpiryDate = DateTime.UtcNow.AddMinutes(30),
            Candidate = new Candidate
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com"
            }
        };

        _repoMock.Setup(r => r.GetByInvitationTokenAsync("token123"))
            .ReturnsAsync(candidateExam);

        // Act
        var result = await _service.VerifyInvitation("token123");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.CandidateId);
        Assert.Equal(10, result.ExamId);
        Assert.Equal("John Doe", result.CandidateName);
        Assert.Equal("john@test.com", result.Email);
    }
}