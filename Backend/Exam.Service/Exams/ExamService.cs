using Exam.Models;
using Exam.Repo;
using Microsoft.Extensions.Logging;
using ExamEntity = Exam.Models.Exam;

namespace Exam.Service
{
    public class ExamService : IExamService
    {
        private readonly IExamRepository _repo;
        private readonly ILogger<ExamService> _logger;

        public ExamService(IExamRepository repo, IQuestionRepository questionRepo, ILogger<ExamService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<ExamResponse> CreateExamAsync(CreateExamRequest dto)
        {
            try
            {
                var exam = new ExamEntity
                {
                    Title = dto.Title.Trim(),
                    DurationMins = dto.DurationMins,
                    CreatedAt = DateTime.UtcNow,
                    ExamQuestions = dto.QuestionIds.Select(qId => new ExamQuestion
                    {
                        QuestionId = qId
                    }).ToList()
                };

                await _repo.AddAsync(exam);
                var created = await _repo.GetWithQuestionsAndChoicesAsync(exam.Id);
                return MapToFullResponseDto(created!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateExamAsync failed — Title={Title}", dto.Title);
                return new ExamResponse();
            }
        }

        public async Task<ExamResponse?> UpdateExamAsync(int id, UpdateExamRequest request)
        {
            try
            {
                var exam = await _repo.GetByIdAsync(id);
                if (exam is null) return null;

                if (request.Title is not null)
                    exam.Title = request.Title.Trim();

                if (request.DurationMins is not null)
                    exam.DurationMins = request.DurationMins.Value;

                exam.CreatedAt = DateTime.UtcNow;

                await _repo.UpdateAsync(exam, request.QuestionIds);

                var updated = await _repo.GetWithQuestionsAndChoicesAsync(id);
                return MapToFullResponseDto(updated!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateExamAsync failed — ExamId={ExamId}", id);
                return null;
            }
        }

        public async Task<bool> DeleteExamAsync(int id)
        {
            try
            {
                var exam = await _repo.GetByIdAsync(id);
                if (exam is null) return false;

                await _repo.DeleteAsync(exam);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteExamAsync failed — ExamId={ExamId}", id);
                return false;
            }
        }

        public async Task<ExamResponse> AssignQuestionsAsync(int examId, List<int> questionIds)
        {
            try
            {
                var exam = await _repo.GetByIdAsync(examId);

                if(exam == null)
                    return new ExamResponse();

                var alreadyAssigned = exam.ExamQuestions?
                    .Select(eq => eq.QuestionId)
                    .ToHashSet() ?? [];

                var toAssign = questionIds
                    .Where(qId => !alreadyAssigned.Contains(qId))
                    .ToList();

                var examQuestions = toAssign.Select(qId => new ExamQuestion
                {
                    ExamId = examId,
                    QuestionId = qId
                }).ToList();

                await _repo.AssignQuestionsAsync(examQuestions);

                var updated = await _repo.GetWithQuestionsAndChoicesAsync(examId);
                return MapToFullResponseDto(updated!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AssignQuestionsAsync failed — ExamId={ExamId}", examId);
                return new ExamResponse();
            }
        }

        public async Task<IEnumerable<ExamResponse>> GetAllExamsAsync()
        {
            var exams = await _repo.GetAllAsync();
            return exams.Select(MapToResponseDto);
        }

        public async Task<ExamResponse?> GetExamByIdAsync(int id)
        {
            var exam = await _repo.GetByIdAsync(id);
            return exam is null ? null : MapToResponseDto(exam);
        }

        public async Task<ExamResponse?> GetExamWithQuestionsAsync(int id)
        {
            var exam = await _repo.GetWithQuestionsAndChoicesAsync(id);
            if (exam is null) return null;
            return MapToFullResponseDto(exam);
        }

        public async Task<IEnumerable<ExamResponse>> GetAllExamsWithQuestionsAsync()
        {
            var exams = await _repo.GetAllWithQuestionsAndChoicesAsync();
            return exams.Select(MapToFullResponseDto);
        }

        public async Task<ExamResponse?> RemoveQuestionAsync(int examId, int questionId)
        {
            var exam = await _repo.GetByIdAsync(examId);
            if (exam is null) return null;

            await _repo.RemoveQuestionAsync(examId, questionId);

            var updated = await _repo.GetWithQuestionsAndChoicesAsync(examId);
            return MapToFullResponseDto(updated!);
        }

        private static ExamResponse MapToResponseDto(ExamEntity exam) => new()
        {
            Id = exam.Id,
            Title = exam.Title,
            DurationMins = exam.DurationMins,
            CreatedAt = exam.CreatedAt,
            TotalQuestions = exam.ExamQuestions?.Count ?? 0
        };

        private static ExamResponse MapToFullResponseDto(ExamEntity exam) => new()
        {
            Id = exam.Id,
            Title = exam.Title,
            DurationMins = exam.DurationMins,
            CreatedAt = exam.CreatedAt,
            TotalQuestions = exam.ExamQuestions?.Count ?? 0,
            Questions = exam.ExamQuestions?
                .Select(eq => new QuestionInExamResponse
                {
                    Id = eq.Question!.Id,
                    TopicId = eq.Question.TopicId,
                    Text = eq.Question.Text,
                    ImageUrl = eq.Question.ImageUrl,
                    TopicTitle = eq.Question.Topic?.Title ?? string.Empty,
                    Choices = eq.Question.Choices
                        .Select(c => new ChoiceInExamResponse
                        {
                            Id = c.Id,
                            Text = c.Text ?? "",
                            IsCorrect = c.IsCorrect,
                            ImageUrl = c.ImageUrl
                        }).ToList()
                }).ToList() ?? []
        };
    }
}