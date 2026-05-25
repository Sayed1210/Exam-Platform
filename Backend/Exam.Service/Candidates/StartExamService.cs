using Exam.Repo;
using Exam.Models;
using Microsoft.Extensions.Logging;

namespace Exam.Service;

public class StartExamService(
    ICandidateExamRepository candidateExamRepo,
    IExamRepository examRepo,
    ILogger<StartExamService> logger) : IStartExamService
{
    private readonly ICandidateExamRepository _candidateExamRepo = candidateExamRepo;
    private readonly IExamRepository _examRepo = examRepo;
    private readonly ILogger<StartExamService> _logger = logger;

    public async Task<(StartExamResponse? Response, string? Error)> StartExam(int examId, StartExamRequest request)
    {
        try
        {
            var candidateExam = await _candidateExamRepo.GetAsync(request.CandidateId, examId);
            if (candidateExam is null)
                return (null, "Candidate is not assigned to this exam");

            if (candidateExam.Status == ExamStatus.DONE || candidateExam.Status == ExamStatus.EXPIRED)
                return (null, "Candidate cannot start this exam");

            
            if(candidateExam.StartDate > DateTime.UtcNow)
                return (null, "Exam has not started yet");
            
            if(candidateExam.ExpiryDate < DateTime.UtcNow)
                return (null, "Exam has already ended");

            var exam = await _examRepo.GetWithQuestionsAndChoicesAsync(examId);
            if (exam is null)
                return (null, "Exam not found");

            // candidateExam.JoinedAt = DateTime.UtcNow;
            // candidateExam.Status = ExamStatus.IN_PROGRESS;
            // await _candidateExamRepo.SaveAsync(candidateExam);

            if (!candidateExam.JoinedAt.HasValue)
            {
                candidateExam.JoinedAt = DateTime.UtcNow;
                candidateExam.Status = ExamStatus.IN_PROGRESS;

                await _candidateExamRepo.SaveAsync(candidateExam);
            }

            var expiresAt =
                candidateExam.JoinedAt.Value.AddMinutes(exam.DurationMins);

            if (DateTime.UtcNow >= expiresAt)
            {
                candidateExam.Status = ExamStatus.DONE;
                await _candidateExamRepo.SaveAsync(candidateExam);
                return (null, "Exam time expired");
            }

            var remainingSeconds = Math.Max(
                0,
                (int)(expiresAt - DateTime.UtcNow).TotalSeconds
            );

            if (remainingSeconds <= 0)
            {
                candidateExam.Status = ExamStatus.DONE;
                await _candidateExamRepo.SaveAsync(candidateExam);
                return (null, "Exam time expired");
            }
            // 
            return (
                new StartExamResponse
                {
                    RemainingSeconds = remainingSeconds,
                    Questions = exam.ExamQuestions.Select(eq => new CandidateQuestionInExamResponse
                    {
                        Id = eq.Question!.Id,
                        Text = eq.Question.Text,
                        ImageUrl = eq.Question.ImageUrl,
                        Choices = eq.Question.Choices.Select(c => new CandidateChoiceInExamResponse
                        {
                            Id = c.Id,
                            Text = c.Text ?? "",
                            ImageUrl = c.ImageUrl
                        }).ToList()
                    }).ToList()
                },
                null
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "StartExam failed — ExamId={ExamId} CandidateId={CandidateId}",
                examId, request.CandidateId);
            return (null, "An unexpected error occurred");
        }
    }
}
