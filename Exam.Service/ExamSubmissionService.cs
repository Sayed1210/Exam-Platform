// Service responsible for handling exam submission logic
// (saving answers, calculating score & updating status to done)
using Exam.Repo;
using Exam.Models;
namespace Exam.Service;


public class ExamSubmissionService(
    ICandidateAnswerRepository answerRepo,
    ICandidateExamRepository examRepo) : IExamSubmissionService
{
    private readonly ICandidateAnswerRepository _answerRepo = answerRepo;
    private readonly ICandidateExamRepository _examRepo = examRepo;

    // Main workflow: candidate submits exam
    public async Task<(bool Success, string? Error)> SubmitExam(int examId, SubmitExamRequest request)
    {
        // Retrieve CandidateExam (ensures candidate is assigned to this exam)
        var candidateExam = await _examRepo.GetAsync(request.CandidateId, examId);
        if (candidateExam is null)
            return (false, "Candidate is not assigned to this exam");

        // Prevent double submission
        if (candidateExam.Status == ExamStatus.DONE)
            return (false, "Exam already submitted");

        // Map request answers → CandidateAnswer entities
        var answers = request.Answers.Select(a => new CandidateAnswer
        {
            CandidateId = request.CandidateId,
            ExamId = examId,
            QuestionId = a.QuestionId,
            ChoiceId = a.ChoiceId
        }).ToList();

        // Save answers via repository to EF change tracker
        await _answerRepo.AddRangeAsync(answers);

        // Fetch all correct choices in ONE query instead of N queries
        var questionIds = request.Answers.Select(a => a.QuestionId).ToList();
        var correctChoiceIds = await _answerRepo.GetCorrectChoiceIdsAsync(questionIds);

        // Calculate score against the in-memory lookup
        int score = request.Answers.Count(a => correctChoiceIds.Contains(a.ChoiceId));

        // Update CandidateExam result
        candidateExam.Score = score;
        candidateExam.Status = ExamStatus.DONE;

        // Save all changes in one transaction
        await _examRepo.SaveAsync(candidateExam);
        return (true, null);
    }
}