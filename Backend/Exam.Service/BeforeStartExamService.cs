using Exam.Models;
using Exam.Repo;
using Microsoft.Extensions.Logging;

namespace Exam.Service;

public class BeforeStartExamService(
    ICandidateExamRepository candidateExamRepo,
    IExamRepository examRepo,
    ILogger<BeforeStartExamService> logger)
    : IBeforeStartExamService
{
    private readonly ICandidateExamRepository _candidateExamRepo = candidateExamRepo;
    private readonly IExamRepository _examRepo = examRepo;
    private readonly ILogger<BeforeStartExamService> _logger = logger;

    public async Task<(BeforeStartExamResponse? Response, string? Error)>
        GetExamInfo(string token)
    {
        try
        {
            var candidateExam =
                await _candidateExamRepo.GetByInvitationTokenAsync(token);

            if (candidateExam is null)
            {
                _logger.LogWarning("GetExamInfo failed — invalid token={Token}", token);
                return (null, "Invalid exam link");
            }

            if (candidateExam.Status != ExamStatus.PENDING)
            {
                _logger.LogWarning(
                    "GetExamInfo rejected — ExamId={ExamId} CandidateId={CandidateId} Status={Status}",
                    candidateExam.ExamId, candidateExam.CandidateId, candidateExam.Status);
                return (null, "Exam already started or submitted");
            }

            var exam = await _examRepo.GetWithQuestionsAndChoicesAsync(
                candidateExam.ExamId);

            if (exam is null)
            {
                _logger.LogWarning(
                    "GetExamInfo failed — ExamId={ExamId} not found in DB",
                    candidateExam.ExamId);
                return (null, "Exam not found");
            }

            _logger.LogInformation(
                "GetExamInfo succeeded — ExamId={ExamId} CandidateId={CandidateId}",
                candidateExam.ExamId, candidateExam.CandidateId);

            return (
                new BeforeStartExamResponse
                {
                    CandidateId = candidateExam.CandidateId,
                    ExamId = candidateExam.ExamId,
                    Title = exam.Title,
                    DurationMins = exam.DurationMins,
                    TotalQuestions = exam.ExamQuestions.Count,
                    Status = candidateExam.Status.ToString()
                },
                null
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "GetExamInfo threw unexpectedly for Token={Token}", token);
            return (null, "An unexpected error occurred");
        }
    }
}