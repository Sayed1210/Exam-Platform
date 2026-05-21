namespace Exam.Service;

using Exam.Repo;
using Exam.Models;
using Microsoft.Extensions.Logging;

public class CandidateService(
    ICandidateRepository candidateRepository,
    ILogger<CandidateService> logger) : ICandidateService
{
    private readonly ICandidateRepository _candidateRepository = candidateRepository;
    private readonly ILogger<CandidateService> _logger = logger;

    public async Task<PagedResponse<CandidateResponse>> GetAllCandidates(
        int page, int pageSize, string? search, int? status, bool noStatus)
    {
        try
        {
            var totalCount = await _candidateRepository.CountAsync(search, status, noStatus);
            var candidates = await _candidateRepository.GetPagedAsync(page, pageSize, search, status, noStatus);

            var items = candidates.Select(c => {
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

            _logger.LogInformation(
                "GetAllCandidates — Page={Page} PageSize={PageSize} TotalCount={TotalCount}",
                page, pageSize, totalCount);

            return new PagedResponse<CandidateResponse>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "GetAllCandidates threw unexpectedly — Page={Page} PageSize={PageSize}",
                page, pageSize);
            return new PagedResponse<CandidateResponse> { Items = [], TotalCount = 0, Page = page, PageSize = pageSize };
        }
    }

    public async Task<CandidateResponse?> GetCandidateById(int id)
    {
        try
        {
            var candidate = await _candidateRepository.GetByIdAsync(id);

            if (candidate is null)
            {
                _logger.LogWarning("GetCandidateById — CandidateId={CandidateId} not found", id);
                return null;
            }

            return new CandidateResponse
            {
                Id = candidate.Id,
                Email = candidate.Email,
                FirstName = candidate.FirstName,
                LastName = candidate.LastName,
                Phone = candidate.Phone
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetCandidateById threw unexpectedly — CandidateId={CandidateId}", id);
            return null;
        }
    }

    public async Task<CandidateResponse?> GetCandidateByExamId(int examId)
    {
        try
        {
            var candidate = await _candidateRepository.GetCandidateByExamIdAsync(examId);

            if (candidate is null)
            {
                _logger.LogWarning("GetCandidateByExamId — ExamId={ExamId} has no candidate", examId);
                return null;
            }

            return new CandidateResponse
            {
                Id = candidate.Id,
                Email = candidate.Email,
                FirstName = candidate.FirstName,
                LastName = candidate.LastName,
                Phone = candidate.Phone
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetCandidateByExamId threw unexpectedly — ExamId={ExamId}", examId);
            return null;
        }
    }

    public async Task<bool> AddCandidate(CreateCandidateRequest dto)
    {
        try
        {
            var existingCandidate = await _candidateRepository.GetByEmailAsync(dto.Email);

            if (existingCandidate is not null)
            {
                _logger.LogWarning("AddCandidate rejected — Email={Email} already exists", dto.Email);
                return false;
            }

            var candidate = new Candidate
            {
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Phone = dto.Phone
            };

            await _candidateRepository.AddAsync(candidate);
            _logger.LogInformation("AddCandidate succeeded — Email={Email}", dto.Email);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AddCandidate threw unexpectedly — Email={Email}", dto.Email);
            return false;
        }
    }

    public async Task<CandidateDetailResponse?> GetDetailAsync(int candidateId)
    {
        try
        {
            var candidate = await _candidateRepository.GetWithExamsAndAnswersAsync(candidateId);

            if (candidate is null)
            {
                _logger.LogWarning("GetDetailAsync — CandidateId={CandidateId} not found", candidateId);
                return null;
            }

            return new CandidateDetailResponse
            {
                Id = candidate.Id,
                Name = candidate.FirstName + " " + candidate.LastName,
                Email = candidate.Email,
                Phone = candidate.Phone ?? "N/A",
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
                            QuestionImageUrl = ca.Question?.ImageUrl,
                            ChoiceText = ca.Choice?.Text ?? string.Empty,
                            ChoiceImageUrl = ca.Choice?.ImageUrl,
                            IsCorrect = ca.Choice?.IsCorrect ?? false,
                        }).ToList() ?? []
                }).ToList() ?? []
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetDetailAsync threw unexpectedly — CandidateId={CandidateId}", candidateId);
            return null;
        }
    }

    public async Task<List<CandidateResponse>> GetUnassignedCandidates()
    {
        try
        {
            var candidates = await _candidateRepository.GetUnassignedCandidatesAsync();
            _logger.LogInformation("GetUnassignedCandidates — returned {Count} candidates", candidates.Count);

            return candidates.Select(c => new CandidateResponse
            {
                Id = c.Id,
                Email = c.Email,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Phone = c.Phone
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetUnassignedCandidates threw unexpectedly");
            return [];
        }
    }

    public async Task<bool> DeleteCandidate(int id)
    {
        try
        {
            var candidate = await _candidateRepository.GetByIdAsync(id);

            if (candidate is null)
            {
                _logger.LogWarning("DeleteCandidate — CandidateId={CandidateId} not found", id);
                return false;
            }

            await _candidateRepository.DeleteAsync(id);
            _logger.LogInformation("DeleteCandidate succeeded — CandidateId={CandidateId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteCandidate threw unexpectedly — CandidateId={CandidateId}", id);
            return false;
        }
    }
}