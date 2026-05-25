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

            if (request.Answers.Count > 0 && !await AnswersBelongToExamAsync(examId, request.Answers))
                return (false, "One or more questions do not belong to this exam");

            if (request.Answers.Count > 0 && !await ChoicesBelongToQuestionsAsync(request.Answers))
                return (false, "One or more choices do not belong to the selected question");

            foreach (var answer in request.Answers)
            {
                await _answerRepo.AddOrUpdateAsync(new CandidateAnswer
                {
                    CandidateId = request.CandidateId,
                    ExamId = examId,
                    QuestionId = answer.QuestionId,
                    ChoiceId = answer.ChoiceId
                });
            }

            var savedAnswers = await _answerRepo.GetByCandidateExamAsync(request.CandidateId, examId);
            var examQuestionIds = await _examRepo.GetExamQuestionIdsAsync(examId);
            var examQuestionIdSet = examQuestionIds.ToHashSet();
            var scorableAnswers = savedAnswers
                .Where(a => examQuestionIdSet.Contains(a.QuestionId))
                .ToList();

            var questionIds = scorableAnswers.Select(a => a.QuestionId).ToList();
            var correctChoiceIds = await _answerRepo.GetCorrectChoiceIdsAsync(questionIds);

            int correctAnswers = scorableAnswers.Count(a => correctChoiceIds.Contains(a.ChoiceId));

            int totalQuestions = examQuestionIds.Count;

            // Calculate score as percentage (out of 100)
            float score = totalQuestions > 0
                ? MathF.Round((float)correctAnswers / totalQuestions * 100, 2)
                : 0;

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

    public async Task<(bool Success, string? Error)> SaveAnswerAsync(int examId, SaveAnswerRequest request)
    {
        try
        {
            var candidateExam = await _examRepo.GetAsync(request.CandidateId, examId);
            if (candidateExam is null)
                return (false, "Candidate is not assigned to this exam");

            if (candidateExam.Status == ExamStatus.DONE)
                return (false, "Exam already submitted");

            if (!await AnswersBelongToExamAsync(examId, new List<AnswerRequest>
                {
                    new AnswerRequest
                    {
                        QuestionId = request.QuestionId,
                        ChoiceId = request.ChoiceId
                    }
                }))
            {
                return (false, "Question does not belong to this exam");
            }

            if (!await _answerRepo.ChoiceBelongsToQuestionAsync(request.ChoiceId, request.QuestionId))
                return (false, "Choice does not belong to this question");

            await _answerRepo.AddOrUpdateAsync(new CandidateAnswer
            {
                CandidateId = request.CandidateId,
                ExamId = examId,
                QuestionId = request.QuestionId,
                ChoiceId = request.ChoiceId
            });

            return (true, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "SaveAnswerAsync failed — ExamId={ExamId} CandidateId={CandidateId} QuestionId={QuestionId}",
                examId, request.CandidateId, request.QuestionId);
            return (false, "An unexpected error occurred");
        }
    }

    public async Task<(bool Success, string? Error, List<AnswerRequest> Answers)> GetSavedAnswersAsync(int examId, int candidateId)
    {
        try
        {
            var candidateExam = await _examRepo.GetAsync(candidateId, examId);
            if (candidateExam is null)
                return (false, "Candidate is not assigned to this exam", []);

            var examQuestionIds = await _examRepo.GetExamQuestionIdsAsync(examId);
            var examQuestionIdSet = examQuestionIds.ToHashSet();

            var answers = await _answerRepo.GetByCandidateExamAsync(candidateId, examId);
            var response = answers
                .Where(a => examQuestionIdSet.Contains(a.QuestionId))
                .Select(a => new AnswerRequest
                {
                    QuestionId = a.QuestionId,
                    ChoiceId = a.ChoiceId
                })
                .ToList();

            return (true, null, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "GetSavedAnswersAsync failed â€” ExamId={ExamId} CandidateId={CandidateId}",
                examId, candidateId);
            return (false, "An unexpected error occurred", []);
        }
    }

    private async Task<bool> AnswersBelongToExamAsync(int examId, List<AnswerRequest> answers)
    {
        var submittedQuestionIds = answers
            .Select(a => a.QuestionId)
            .Distinct()
            .ToList();

        var examQuestionIds = await _examRepo.GetExamQuestionIdsAsync(examId);
        var examQuestionIdSet = examQuestionIds.ToHashSet();

        return submittedQuestionIds.All(examQuestionIdSet.Contains);
    }

    private async Task<bool> ChoicesBelongToQuestionsAsync(List<AnswerRequest> answers)
    {
        foreach (var answer in answers)
        {
            if (!await _answerRepo.ChoiceBelongsToQuestionAsync(answer.ChoiceId, answer.QuestionId))
                return false;
        }

        return true;
    }
}
