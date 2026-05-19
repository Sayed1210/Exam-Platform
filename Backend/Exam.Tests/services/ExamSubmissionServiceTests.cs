using Moq;
using Xunit;
using Exam.Service;
using Exam.Repo;
using Exam.Models;
using Microsoft.Extensions.Logging.Abstractions;

public class ExamSubmissionServiceTests
{
    private readonly Mock<ICandidateAnswerRepository> _answerRepoMock;
    private readonly Mock<ICandidateExamRepository> _examRepoMock;
    private readonly ExamSubmissionService _service;

    public ExamSubmissionServiceTests()
    {
        _answerRepoMock = new Mock<ICandidateAnswerRepository>();
        _examRepoMock = new Mock<ICandidateExamRepository>();

        _service = new ExamSubmissionService(
            _answerRepoMock.Object,
            _examRepoMock.Object,
            NullLogger<ExamSubmissionService>.Instance
        );
    }

    [Fact]
    public async Task SubmitExam_ShouldThrow_WhenCandidateNotAssigned()
    {
        var request = new SubmitExamRequest
        {
            CandidateId = 1,
            Answers = new List<AnswerRequest>()
        };

        _examRepoMock
            .Setup(r => r.GetAsync(1, 10))
            .ReturnsAsync((CandidateExam?)null);

        var result = await _service.SubmitExam(10, request);

        Assert.False(result.Success);
        Assert.Equal("Candidate is not assigned to this exam", result.Error);
    }

    [Fact]
    public async Task SubmitExam_ShouldThrow_WhenExamAlreadySubmitted()
    {
        var request = new SubmitExamRequest
        {
            CandidateId = 1,
            Answers = new List<AnswerRequest>()
        };

        var candidateExam = new CandidateExam
        {
            CandidateId = 1,
            ExamId = 10,
            Status = ExamStatus.DONE,
            InvitationToken = "test-token"
        };

        _examRepoMock
            .Setup(r => r.GetAsync(1, 10))
            .ReturnsAsync(candidateExam);

        var result = await _service.SubmitExam(10, request);

        Assert.False(result.Success);
        Assert.Equal("Exam already submitted", result.Error);
    }

    [Fact]
    public async Task SubmitExam_ShouldRejectAnswers_WhenQuestionDoesNotBelongToExam()
    {
        var request = new SubmitExamRequest
        {
            CandidateId = 1,
            Answers = new List<AnswerRequest>
            {
                new AnswerRequest { QuestionId = 1, ChoiceId = 5 },
                new AnswerRequest { QuestionId = 99, ChoiceId = 8 }
            }
        };

        var candidateExam = new CandidateExam
        {
            CandidateId = 1,
            ExamId = 10,
            Status = ExamStatus.PENDING,
            InvitationToken = "test-token"
        };

        _examRepoMock
            .Setup(r => r.GetAsync(1, 10))
            .ReturnsAsync(candidateExam);

        _examRepoMock
            .Setup(r => r.GetExamQuestionIdsAsync(10))
            .ReturnsAsync(new List<int> { 1, 2 });

        var result = await _service.SubmitExam(10, request);

        Assert.False(result.Success);
        Assert.Equal("One or more questions do not belong to this exam", result.Error);
        _answerRepoMock.Verify(r => r.AddRangeAsync(It.IsAny<List<CandidateAnswer>>()), Times.Never);
        _examRepoMock.Verify(r => r.SaveAsync(It.IsAny<CandidateExam>()), Times.Never);
    }

    [Fact]
    public async Task SubmitExam_ShouldSaveAnswers()
    {
        var request = new SubmitExamRequest
        {
            CandidateId = 1,
            Answers = new List<AnswerRequest>
            {
                new AnswerRequest { QuestionId = 1, ChoiceId = 5 }
            }
        };

        var candidateExam = new CandidateExam
        {
            CandidateId = 1,
            ExamId = 10,
            Status = ExamStatus.PENDING,
            InvitationToken = "test-token"
        };

        _examRepoMock
            .Setup(r => r.GetAsync(1, 10))
            .ReturnsAsync(candidateExam);

        _examRepoMock
            .Setup(r => r.GetExamQuestionIdsAsync(10))
            .ReturnsAsync(new List<int> { 1 });

        _answerRepoMock
            .Setup(r => r.GetCorrectChoiceIdsAsync(It.Is<List<int>>(ids => ids.SequenceEqual(new List<int> { 1 }))))
            .ReturnsAsync(new List<int> { 5 });

        await _service.SubmitExam(10, request);

        _answerRepoMock.Verify(r =>
            r.AddRangeAsync(It.IsAny<List<CandidateAnswer>>()),
            Times.Once);
    }

    [Fact]
    public async Task SubmitExam_ShouldCalculateScoreCorrectly()
    {
        var request = new SubmitExamRequest
        {
            CandidateId = 1,
            Answers = new List<AnswerRequest>
            {
                new AnswerRequest { QuestionId = 1, ChoiceId = 5 },
                new AnswerRequest { QuestionId = 2, ChoiceId = 8 }
            }
        };

        var candidateExam = new CandidateExam
        {
            CandidateId = 1,
            ExamId = 10,
            Status = ExamStatus.PENDING,
            InvitationToken = "test-token"
        };

        _examRepoMock
            .Setup(r => r.GetAsync(1, 10))
            .ReturnsAsync(candidateExam);

        _examRepoMock
            .Setup(r => r.GetExamQuestionIdsAsync(10))
            .ReturnsAsync(new List<int> { 1, 2 });

        _answerRepoMock
            .Setup(r => r.GetCorrectChoiceIdsAsync(It.Is<List<int>>(ids => ids.SequenceEqual(new List<int> { 1, 2 }))))
            .ReturnsAsync(new List<int> { 5, 9 });

        await _service.SubmitExam(10, request);

        Assert.Equal(1, candidateExam.Score);
        Assert.Equal(ExamStatus.DONE, candidateExam.Status);
    }

    [Fact]
    public async Task SubmitExam_ShouldSaveExamResult()
    {
        var request = new SubmitExamRequest
        {
            CandidateId = 1,
            Answers = new List<AnswerRequest>
            {
                new AnswerRequest { QuestionId = 1, ChoiceId = 5 }
            }
        };

        var candidateExam = new CandidateExam
        {
            CandidateId = 1,
            ExamId = 10,
            Status = ExamStatus.PENDING,
            InvitationToken = "test-token"
        };

        _examRepoMock
            .Setup(r => r.GetAsync(1, 10))
            .ReturnsAsync(candidateExam);

        _examRepoMock
            .Setup(r => r.GetExamQuestionIdsAsync(10))
            .ReturnsAsync(new List<int> { 1 });

        _answerRepoMock
            .Setup(r => r.GetCorrectChoiceIdsAsync(It.Is<List<int>>(ids => ids.SequenceEqual(new List<int> { 1 }))))
            .ReturnsAsync(new List<int> { 5 });

        await _service.SubmitExam(10, request);

        _examRepoMock.Verify(r =>
            r.SaveAsync(candidateExam),
            Times.Once);
    }
}
