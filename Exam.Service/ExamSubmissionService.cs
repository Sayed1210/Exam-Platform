// Service responsible for handling exam submission logic
// (saving answers, calculating score & updating status to done)
using Exam.Repo;
using Exam.Models;

public class ExamSubmissionService(
    ICandidateAnswerRepository answerRepo,
    ICandidateExamRepository examRepo) : IExamSubmissionService
{
    private readonly ICandidateAnswerRepository _answerRepo = answerRepo;
    private readonly ICandidateExamRepository _examRepo = examRepo;

    // Main workflow: candidate submits exam
    public async Task SubmitExam(int examId, SubmitExamRequest request)
    {
        // Retrieve CandidateExam (ensures candidate is assigned to this exam)
        var candidateExam = await _examRepo.GetAsync(
            request.CandidateId, examId)
            ?? throw new Exception("Candidate is not assigned to this exam");

        // Prevent double submission
        if (candidateExam.Status == ExamStatus.DONE)
            throw new Exception("Exam already submitted");

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

        // Calculate score
        int score = 0;
        foreach (var answer in request.Answers)
        {
            var correctChoiceIds = await _answerRepo.GetCorrectChoiceIdsAsync(answer.QuestionId);

            if (correctChoiceIds.Contains(answer.ChoiceId))
                score++;
        }

        // Update CandidateExam result
        candidateExam.Score = score;
        candidateExam.Status = ExamStatus.DONE;

        // Save all changes in one transaction
        await _examRepo.SaveAsync(candidateExam);
    }
}