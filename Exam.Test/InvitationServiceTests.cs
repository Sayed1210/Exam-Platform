using Moq;
using Xunit;
using Exam.Service;
using Exam.Repo;
using Exam.Models;
using Exam.Models.Dtos.Requests;
using Microsoft.EntityFrameworkCore.Storage;
namespace Exam.Test;

public class InvitationServiceTests
{
    private readonly Mock<ICandidateExamRepository> _repoMock;
    private readonly Mock<IEmailService> _emailMock;
    private readonly InvitationService _service;

    public InvitationServiceTests()
    {
        _repoMock = new Mock<ICandidateExamRepository>();
        _emailMock = new Mock<IEmailService>();

        _service = new InvitationService(_repoMock.Object, _emailMock.Object);
    }
    [Fact]
    public async Task SendInvitationAsync_CandidateNotFound_ReturnsFailure()
    {
    
    var request = new SendInvitationRequest(1, "nonexistent@gmail.com", "First", "Last");
    _repoMock.Setup(r => r.GetCandidateByEmailAsync(It.IsAny<string>()))
             .ReturnsAsync((Candidate?)null);


    var result = await _service.SendInvitationAsync(request);

    Assert.False(result.IsSuccess);
    Assert.Equal("Candidate with this email not found", result.Message);
    
    
    _emailMock.Verify(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(),It.IsAny<string>()), Times.Never);
    }
    [Fact]
    public async Task SendInvitationAsync_EmailFails_RollsBackDatabase()
    {
    var request = new SendInvitationRequest(1, "test@gmail.com", "First", "Last");
    var candidate = new Candidate { Id = 1, Email = "test@gmail.com" };
    var transactionMock = new Mock<IDbContextTransaction>();

    _repoMock.Setup(r => r.GetCandidateByEmailAsync(It.IsAny<string>())).ReturnsAsync(candidate);
    _repoMock.Setup(r => r.BeginTransactionAsync()).ReturnsAsync(transactionMock.Object);

    
    _emailMock.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(),It.IsAny<string>()))
              .ThrowsAsync(new Exception("SMTP Server Down"));


    var result = await _service.SendInvitationAsync(request);

    Assert.False(result.IsSuccess);
    Assert.Contains("rolled back", result.Message);

    
    transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task SendInvitationAsync_ValidCandidate_ReturnsSuccess()
    {
        var request = new SendInvitationRequest (
    1,               
    "test@gmail.com",   
    "First Name",       
    "Last Name"         
      );
        var candidate = new Candidate { Id = 101, Email = "test@enozom.com" };
        var transactionMock = new Mock<IDbContextTransaction>();
        _repoMock.Setup(r => r.GetCandidateByEmailAsync(request.Email))
                 .ReturnsAsync(candidate);
        
        _repoMock.Setup(r => r.BeginTransactionAsync())
                 .ReturnsAsync(transactionMock.Object);

        var result = await _service.SendInvitationAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal("Invitation sent successfully.", result.Message);
        

        _emailMock.Verify(e => e.SendEmailAsync(request.Email, It.IsAny<string>(),It.IsAny<string>()), Times.Once);
        transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}