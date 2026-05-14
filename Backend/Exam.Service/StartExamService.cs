// Service responsible for handling starting exam logic
// (get list of exam questions using exam id, update joined at, update status to in progress)
using Exam.Repo;
using Exam.Models;
namespace Exam.Service;

public class StartExamService(
    ICandidateExamRepository candidateExamRepo, 
    IExamRepository examRepo
    ) : IStartExamService
{
    private readonly ICandidateExamRepository _candidateExamRepo = candidateExamRepo;
    private readonly IExamRepository _examRepo = examRepo;

    // Main workflow: candidate starts exam
    public async Task<(StartExamResponse? Response, string? Error)> StartExam(int examId, StartExamRequest request)
    {
        // Retrieve CandidateExam (ensures candidate is assigned to this exam)
        var candidateExam = await _candidateExamRepo.GetAsync(request.CandidateId, examId);
        if (candidateExam is null)
            return (null, "Candidate is not assigned to this exam");
        if (candidateExam.Status != ExamStatus.PENDING)
            return (null, "Candidated cannot start this exam");

        // Retrieve exam with questions and choices to ensure it exists and is ready
        var exam = await _examRepo.GetWithQuestionsAndChoicesAsync(examId);

        if (exam is null)         
            return (null, "Exam not found");

        // Update joined at and status
        candidateExam.JoinedAt = DateTime.UtcNow;
        candidateExam.Status = ExamStatus.IN_PROGRESS;

        // Save changes via repository
        await _candidateExamRepo.SaveAsync(candidateExam);

        // return success and response data
        return (
            new StartExamResponse 
            {
                Questions = exam.ExamQuestions.Select(eq => new QuestionInExamResponse
                {
                    Id = eq.Question!.Id,
                    Text = eq.Question.Text,
                    ImageUrl = eq.Question.ImageUrl,
                    Choices = eq.Question.Choices.Select(c => new ChoiceInExamResponse
                    {
                        Id = c.Id,
                        Text = c.Text,
                        IsCorrect = c.IsCorrect,
                        ImageUrl = c.ImageUrl
                    }).ToList()
                }).ToList()
            }
            , null
        );
    }
}