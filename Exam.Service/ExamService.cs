using Exam.Models;
using Exam.Models.dtos.requests;
using Exam.Models.dtos.responses;
using Exam.Repo;
using ExamEntity = Exam.Models.Exam;

namespace Exam.Service
{
    public class ExamService : IExamService
    {
        private readonly IExamRepository _repo;

        public ExamService(IExamRepository repo, IQuestionRepository questionRepo)
        {
            _repo = repo;
        }

        public async Task<ExamResponse> CreateExamAsync(CreateExamRequest dto)
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

        public async Task<ExamResponse?> UpdateExamAsync(int id, UpdateExamRequest request)
        {
            var exam = await _repo.GetByIdAsync(id);
            if (exam is null) return null;

            // Only update fields that were explicitly provided
            if (request.Title is not null)
                exam.Title = request.Title.Trim();

            if (request.DurationMins is not null)
                exam.DurationMins = request.DurationMins.Value;
           
            exam.CreatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(exam, request.QuestionIds);  

            var updated = await _repo.GetWithQuestionsAndChoicesAsync(id);
            return MapToFullResponseDto(updated!);
        }

        public async Task<bool> DeleteExamAsync(int id)
        {
            var exam = await _repo.GetByIdAsync(id);
            if (exam is null) return false;

            await _repo.DeleteAsync(exam);
            return true;
        }

        public async Task<ExamResponse> AssignQuestionsAsync(int examId, List<int> questionIds)
        {
          

            var exam = await _repo.GetByIdAsync(examId);
           

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
                    Text = eq.Question.Text,
                    ImageUrl = eq.Question.ImageUrl,
                    Choices = eq.Question.Choices
                        .Select(c => new ChoiceInExamResponse
                        {
                            Id = c.Id,
                            Text = c.Text,
                            IsCorrect = c.IsCorrect,
                            ImageUrl = c.ImageUrl
                        }).ToList()
                }).ToList() ?? []
        };
    }
}
