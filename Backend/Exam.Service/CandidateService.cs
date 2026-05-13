namespace Exam.Service;
using Exam.Repo;
using Exam.Models;

// Constructor injection (Primary Constructor)
// The repository is injected by DI & stored in a private field
public class CandidateService(ICandidateRepository candidateRepository) : ICandidateService
{
     // Store injected repository for use in methods
    private readonly ICandidateRepository _candidateRepository = candidateRepository;

public async Task<List<CandidateResponse>> GetAllCandidates()
{
    var candidates = await _candidateRepository.GetAllAsync();

    return candidates.Select(c => {
        var latestExam = c.CandidateExams?
            .OrderByDescending(e => e.InvitedAt)
            .FirstOrDefault();

        return new CandidateResponse
        {
            Id = c.Id,
            Email = c.Email,
            FirstName = c.FirstName,
            LastName = c.LastName,
            Phone = c.Phone,
            Score = latestExam?.Score,
            Status = latestExam?.Status,
            InvitedAt = latestExam?.InvitedAt,
            StartedAt = latestExam?.JoinedAt
        };
    }).ToList();
}

    public async Task<CandidateResponse?> GetCandidateById(int id)
    {
        // Ask repository for entity from database
        var candidate = await _candidateRepository.GetByIdAsync(id);

        if (candidate == null)
            return null; // throw new Exception($"Candidate with id {id} not found");

        // Map Entity → DTO
        return new CandidateResponse
        {
            Id = candidate.Id,
            Email = candidate.Email,
            FirstName = candidate.FirstName,
            LastName = candidate.LastName,
            Phone = candidate.Phone
        };
    }

    public async Task<CandidateResponse?> GetCandidateByExamId(int examId)
    {
        var candidate = await _candidateRepository.GetCandidateByExamIdAsync(examId);

        if (candidate == null)
            return null; // throw new Exception($"Candidate with exam id {examId} not found");

        return new CandidateResponse
        {
            Id = candidate.Id,
            Email = candidate.Email,
            FirstName = candidate.FirstName,
            LastName = candidate.LastName,
            Phone = candidate.Phone
        };
    }

    public async Task<bool> AddCandidate(CreateCandidateRequest dto)
    {
        // Check if candidate already exists
        var existingCandidate = await _candidateRepository.GetByEmailAsync(dto.Email);

        if (existingCandidate != null)
            return false; // throw new Exception("Candidate already exists");
        
        // Map DTO → Entity
        var candidate = new Candidate
        {
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Phone = dto.Phone
        };

        // Send entity to repository to be saved in DB
        await _candidateRepository.AddAsync(candidate);
        return true;
    }
    public async Task<CandidateDetailResponse?> GetDetailAsync(int candidateId)
{
    var candidate = await _candidateRepository.GetWithExamsAndAnswersAsync(candidateId);
    if (candidate is null) return null;

    return new CandidateDetailResponse
    {
        Id = candidate.Id,
        Name = candidate.FirstName + " " + candidate.LastName,
        Email = candidate.Email,
        Phone= candidate.Phone,
        Exams = candidate.CandidateExams?.Select(ce => new CandidateExamDetailResponse
        {
           
            ExamTitle = ce.Exam?.Title ?? string.Empty,
            InvitedAt = ce.InvitedAt,
            StartedAt = ce.JoinedAt,
            Status = ce.Status,
            Score = ce.Score,
            Answers = candidate.CandidateAnswers?
                .Where(ca => ca.ExamId == ce.ExamId)
                .Select(ca => new CandidateAnswerDetail
                {
                    QuestionText = ca.Question?.Text ?? string.Empty,
                    ChoiceText = ca.Choice?.Text ?? string.Empty
                
                }).ToList() ?? []
        }).ToList() ?? []
    };
}

    public async Task<bool> DeleteCandidate(int id)
    {
        // Check if candidate exists first
        var candidate = await _candidateRepository.GetByIdAsync(id);

        if (candidate == null)
            return false; // throw new Exception($"Candidate with id {id} not found");

        // Delegate deletion to repository
        await _candidateRepository.DeleteAsync(id);
        return true;
    }
}

