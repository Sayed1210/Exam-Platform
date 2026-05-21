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

            if (!await AnswersBelongToExamAsync(examId, request.Answers))
                return (false, "One or more questions do not belong to this exam");

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

            int correctAnswers = request.Answers.Count(a => correctChoiceIds.Contains(a.ChoiceId));

            // Get total questions in the exam
            var totalExamQuestionIds = await _examRepo.GetExamQuestionIdsAsync(examId);
            int totalQuestions = totalExamQuestionIds.Count;
            Console.WriteLine("111111111111111111111111111111111111");
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
}
