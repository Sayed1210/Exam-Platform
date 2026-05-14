using Exam.Models;
using Exam.Repo;

namespace Exam.Service;

public class BeforeStartExamService(
    ICandidateExamRepository candidateExamRepo,
    IExamRepository examRepo)
    : IBeforeStartExamService
{
    private readonly ICandidateExamRepository _candidateExamRepo = candidateExamRepo;
    private readonly IExamRepository _examRepo = examRepo;

    public async Task<(BeforeStartExamResponse? Response, string? Error)>
        GetExamInfo(string token)
    {
        var candidateExam =
            await _candidateExamRepo.GetByInvitationTokenAsync(token);

        if (candidateExam is null)
            return (null, "Invalid exam link");

        if (candidateExam.Status != ExamStatus.PENDING)
            return (null, "Exam already started or submitted");

        var exam = await _examRepo.GetWithQuestionsAndChoicesAsync(
            candidateExam.ExamId);

        if (exam is null)
            return (null, "Exam not found");

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
}