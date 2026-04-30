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
        // Fetch all Candidate entities from database
        var candidates = await _candidateRepository.GetAllAsync();

        // Map Entity → DTO
        return candidates.Select(c => new CandidateResponse
        {
            Id = c.Id,
            Email = c.Email,
            FirstName = c.FirstName,
            LastName = c.LastName,
            Phone = c.Phone
        }).ToList();
    }

    public async Task<CandidateResponse?> GetCandidateById(int id)
    {
        // Ask repository for entity from database
        var candidate = await _candidateRepository.GetByIdAsync(id);

        if (candidate == null)
            throw new Exception($"Candidate with id {id} not found"); // return null;

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
            throw new Exception($"Candidate with exam id {examId} not found");

        return new CandidateResponse
        {
            Id = candidate.Id,
            Email = candidate.Email,
            FirstName = candidate.FirstName,
            LastName = candidate.LastName,
            Phone = candidate.Phone
        };
    }

    public async Task AddCandidate(CreateCandidate dto)
    {
        // Check if candidate already exists
        var existingCandidate = await _candidateRepository.GetByEmailAsync(dto.Email);

        if (existingCandidate != null)
            throw new Exception("Candidate already exists");
        
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
    }

    public async Task DeleteCandidate(int id)
    {
        // Check if candidate exists first
        var candidate = await _candidateRepository.GetByIdAsync(id);

        if (candidate == null)
            throw new Exception($"Candidate with id {id} not found");

        // Delegate deletion to repository
        await _candidateRepository.DeleteAsync(id);
    }
}

