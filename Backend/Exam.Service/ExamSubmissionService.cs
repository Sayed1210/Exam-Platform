using Exam.Repo;
using Exam.Models;
using Microsoft.Extensions.Logging;

namespace Exam.Service;

public class ExamSubmissionService(
    ICandidateAnswerRepository answerRepo,
    ICandidateExamRepository examRepo,
    ILogger<ExamSubmissionService> logger) : IExamSubmissionService
{
    private readonly ICandidateAnswerRepository _answerRepo = answerRepo;
    private readonly ICandidateExamRepository _examRepo = examRepo;
    private readonly ILogger<ExamSubmissionService> _logger = logger;

    public async Task<(bool Success, string? Error)> SubmitExam(int examId, SubmitExamRequest request)
    {
        try
        {
            var candidateExam = await _examRepo.GetAsync(request.CandidateId, examId);
            if (candidateExam is null)
                return (false, "Candidate is not assigned to this exam");

            if (candidateExam.Status == ExamStatus.DONE)
                return (false, "Exam already submitted");

            var answers = request.Answers.Select(a => new CandidateAnswer
            {
                CandidateId = request.CandidateId,
                ExamId = examId,
                QuestionId = a.QuestionId,
                ChoiceId = a.ChoiceId
            }).ToList();

            await _answerRepo.AddRangeAsync(answers);

            var questionIds = request.Answers.Select(a => a.QuestionId).ToList();
            var correctChoiceIds = await _answerRepo.GetCorrectChoiceIdsAsync(questionIds);

            int score = request.Answers.Count(a => correctChoiceIds.Contains(a.ChoiceId));

            candidateExam.Score = score;
            candidateExam.Status = ExamStatus.DONE;

            await _examRepo.SaveAsync(candidateExam);
            return (true, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "SubmitExam failed — ExamId={ExamId} CandidateId={CandidateId}",
                examId, request.CandidateId);
            return (false, "An unexpected error occurred");
        }
    }
}