using ExamApi.DTOs.Requests;
using ExamApi.DTOs.Responses;
using ExamApi.Repositories;

namespace ExamApi.Services
{
    public class ExamService : IExamService
    {
        private readonly IExamRepository _repo;
        private readonly IQuestionRepository _questionRepo;

        public ExamService(IExamRepository repo, IQuestionRepository questionRepo)
        {
            _repo = repo;
            _questionRepo = questionRepo;
        }

        // ── CREATE ────────────────────────────────────────────────────
        public async Task<ExamResponseDto> CreateExamAsync(CreateExamDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("Exam title cannot be empty.", nameof(dto.Title));

            if (dto.Title.Length > 200)
                throw new ArgumentException("Exam title cannot exceed 200 characters.", nameof(dto.Title));

            if (dto.DurationMins <= 0)
                throw new ArgumentException("Duration must be greater than 0 minutes.", nameof(dto.DurationMins));

            var exam = new Exam
            {
                Title = dto.Title.Trim(),
                DurationMins = dto.DurationMins,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime
            };

            await _repo.AddAsync(exam);
            return MapToResponseDto(exam);
        }

        // ── GET ALL ───────────────────────────────────────────────────
        public async Task<IEnumerable<ExamResponseDto>> GetAllExamsAsync()
        {
            var exams = await _repo.GetAllAsync();
            return exams.Select(MapToResponseDto);
        }

        // ── GET BY ID ─────────────────────────────────────────────────
        public async Task<ExamResponseDto?> GetExamByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be a positive number.", nameof(id));

            var exam = await _repo.GetByIdAsync(id);
            return exam is null ? null : MapToResponseDto(exam);
        }

        // ── GET EXAM WITH QUESTIONS + CHOICES ─────────────────────────
        public async Task<ExamResponseDto?> GetExamWithQuestionsAsync(int id)
        {
            // Validate: id must be positive
            if (id <= 0)
                throw new ArgumentException("Id must be a positive number.", nameof(id));

            // Fetch exam → ExamQuestions → Question → Choices in one query
            var exam = await _repo.GetWithQuestionsAndChoicesAsync(id);
            if (exam is null) return null;

            return new ExamResponseDto
            {
                Id = exam.Id,
                Title = exam.Title,
                DurationMins = exam.DurationMins,
                StartTime = exam.StartTime,
                EndTime = exam.EndTime,
                TotalQuestions = exam.ExamQuestions?.Count ?? 0,
                Questions = exam.ExamQuestions?
                    .Select(eq => new QuestionInExamDto
                    {
                        Id = eq.Question!.Id,
                        Text = eq.Question.Text,
                        ImageUrl = eq.Question.ImageUrl,
                        Choices = eq.Question.Choices
                            .Select(c => new ChoiceInExamDto
                            {
                                Id = c.Id,
                                Text = c.Text,
                                IsCorrect = c.IsCorrect,
                                ImageUrl = c.ImageUrl
                            }).ToList()
                    }).ToList() ?? []
            };
        }

        // ── UPDATE ────────────────────────────────────────────────────
        public async Task<ExamResponseDto?> UpdateExamAsync(int id, CreateExamDto dto)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be a positive number.", nameof(id));

            ArgumentNullException.ThrowIfNull(dto);

            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("Exam title cannot be empty.", nameof(dto.Title));

            if (dto.Title.Length > 200)
                throw new ArgumentException("Exam title cannot exceed 200 characters.", nameof(dto.Title));

            if (dto.DurationMins <= 0)
                throw new ArgumentException("Duration must be greater than 0 minutes.", nameof(dto.DurationMins));

            var exam = await _repo.GetByIdAsync(id);
            if (exam is null) return null;

            exam.Title = dto.Title.Trim();
            exam.DurationMins = dto.DurationMins;
            exam.StartTime = dto.StartTime;
            exam.EndTime = dto.EndTime;

            await _repo.UpdateAsync(exam);
            return MapToResponseDto(exam);
        }

        // ── DELETE ────────────────────────────────────────────────────
        public async Task<bool> DeleteExamAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be a positive number.", nameof(id));

            var exam = await _repo.GetByIdAsync(id);
            if (exam is null) return false;

            await _repo.DeleteAsync(exam);
            return true;
        }

        // ── ASSIGN QUESTIONS ──────────────────────────────────────────
        public async Task<ExamResponseDto> AssignQuestionsAsync(int examId, List<int> questionIds)
        {
            if (examId <= 0)
                throw new ArgumentException("ExamId must be a positive number.", nameof(examId));

            if (questionIds is null || questionIds.Count == 0)
                throw new ArgumentException("Question list cannot be empty.", nameof(questionIds));

            var duplicates = questionIds
                .GroupBy(id => id)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicates.Count > 0)
                throw new ArgumentException(
                    $"Duplicate question ids in request: {string.Join(", ", duplicates)}.",
                    nameof(questionIds));

            var exam = await _repo.GetByIdAsync(examId);
            if (exam is null)
                throw new KeyNotFoundException($"Exam {examId} not found.");

            var notFound = new List<int>();
            foreach (var qId in questionIds)
            {
                var exists = await _questionRepo.ExistsAsync(qId);
                if (!exists) notFound.Add(qId);
            }

            if (notFound.Count > 0)
                throw new KeyNotFoundException(
                    $"Questions not found: {string.Join(", ", notFound)}.");

            var alreadyAssigned = exam.ExamQuestions?
                .Select(eq => eq.QuestionId)
                .ToHashSet() ?? [];

            var toAssign = questionIds
                .Where(qId => !alreadyAssigned.Contains(qId))
                .ToList();

            if (toAssign.Count == 0)
                throw new InvalidOperationException(
                    "All provided questions are already assigned to this exam.");

            var examQuestions = toAssign.Select(qId => new ExamQuestion
            {
                ExamId = examId,
                QuestionId = qId
            }).ToList();

            await _repo.AssignQuestionsAsync(examQuestions);

            // ✅ Reload so TotalQuestions reflects the new count
            var updated = await _repo.GetByIdAsync(examId);
            return MapToResponseDto(updated!);
        }

        // ── REMOVE QUESTION ───────────────────────────────────────────
        public async Task<ExamResponseDto> RemoveQuestionAsync(int examId, int questionId)
        {
            if (examId <= 0)
                throw new ArgumentException("ExamId must be a positive number.", nameof(examId));
            if (questionId <= 0)
                throw new ArgumentException("QuestionId must be a positive number.", nameof(questionId));

            var exam = await _repo.GetByIdAsync(examId);
            if (exam is null)
                throw new KeyNotFoundException($"Exam {examId} not found.");

            var assigned = exam.ExamQuestions?
                .Any(eq => eq.QuestionId == questionId) ?? false;

            if (!assigned)
                throw new KeyNotFoundException(
                    $"Question {questionId} is not assigned to exam {examId}.");

            await _repo.RemoveQuestionAsync(examId, questionId);

            // ✅ Reload so TotalQuestions reflects the updated count
            var updated = await _repo.GetByIdAsync(examId);
            return MapToResponseDto(updated!);
        }

        // ── HELPER — Map Entity to DTO ────────────────────────────────
        private static ExamResponseDto MapToResponseDto(Exam exam) => new()
        {
            Id = exam.Id,
            Title = exam.Title,
            DurationMins = exam.DurationMins,
            StartTime = exam.StartTime,
            EndTime = exam.EndTime,
            TotalQuestions = exam.ExamQuestions?.Count ?? 0  // ✅ always live count
        };
    }
}