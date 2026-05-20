using Exam.Repo;
using Exam.Models;
using Microsoft.Extensions.Logging;

namespace Exam.Service
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _repo;
        private readonly ILogger<QuestionService> _logger;

        public QuestionService(IQuestionRepository repo, ILogger<QuestionService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<PagedResponse<QuestionResponse>> GetAllAsync(
            int page, int pageSize, string? search = null, int[]? topicIds = null)
        {
            var (items, totalCount) = await _repo.GetPagedAsync(page, pageSize, search, topicIds);

            return new PagedResponse<QuestionResponse>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = items.Select(MapToResponse).ToList()
            };
        }

        public async Task<IEnumerable<QuestionResponse>> GetByTopicIdAsync(int topicId)
        {
            var questions = await _repo.GetByTopicIdAsync(topicId);
            return questions.Select(MapToResponse);
        }

        public async Task<QuestionResponse?> GetByIdAsync(int id)
        {
            var question = await _repo.GetByIdAsync(id);
            return question is null ? null : MapToResponse(question);
        }

        public async Task<QuestionResponse> CreateAsync(QuestionRequest request)
        {
            try
            {
                var question = new Question
                {
                    TopicId = request.TopicId,
                    Text = request.Text.Trim(),
                    ImageUrl = request.ImageUrl,
                    Choices = request.Choices.Select(c => new Choice
                    {
                        Text = c.Text?.Trim(),
                        IsCorrect = c.IsCorrect,
                        ImageUrl = c.ImageUrl
                    }).ToList()
                };

                var created = await _repo.CreateAsync(question);
                var result = await _repo.GetByIdAsync(created.Id);
                return MapToResponse(result!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateAsync failed — TopicId={TopicId}", request.TopicId);
                return new QuestionResponse();
            }
        }

        public async Task<QuestionResponse?> UpdateAsync(int id, UpdateQuestionRequest request)
        {
            try
            {
                var question = await _repo.GetByIdAsync(id);
                if (question is null) return null;

                if (request.Text is not null)
                    question.Text = request.Text.Trim();

                if (request.TopicId > 0)
                    question.TopicId = request.TopicId;

                if (request.ImageUrl is not null)
                    question.ImageUrl = request.ImageUrl;

                var updateChoices = request.Choices is not null;
                if (updateChoices)
                    question.Choices = request.Choices!.Select(c => new Choice
                    {
                        QuestionId = id,
                        Text = c.Text?.Trim(),
                        IsCorrect = c.IsCorrect,
                        ImageUrl = c.ImageUrl
                    }).ToList();

                var result = await _repo.UpdateAsync(question, updateChoices);
                return MapToResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync failed — QuestionId={QuestionId}", id);
                return null;
            }
        }

        public async Task<QuestionResponse?> UpdateChoiceAsync(int questionId, int choiceId, ChoiceRequest request)
        {
            try
            {
                var question = await _repo.GetByIdAsync(questionId);
                if (question is null) return null;

                var choice = question.Choices.FirstOrDefault(c => c.Id == choiceId);
                if (choice is null) return null;

                choice.Text = request.Text?.Trim();
                choice.IsCorrect = request.IsCorrect;
                choice.ImageUrl = request.ImageUrl;

                var result = await _repo.UpdateChoiceAsync(choice);
                return MapToResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "UpdateChoiceAsync failed — QuestionId={QuestionId} ChoiceId={ChoiceId}",
                    questionId, choiceId);
                return null;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var question = await _repo.GetByIdAsync(id);
                if (question is null) return false;

                await _repo.DeleteAsync(question);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync failed — QuestionId={QuestionId}", id);
                return false;
            }
        }

        private static QuestionResponse MapToResponse(Question q) => new()
        {
            Id = q.Id,
            TopicId = q.TopicId,
            TopicTitle = q.Topic?.Title ?? string.Empty,
            Text = q.Text,
            ImageUrl = q.ImageUrl,
            Choices = q.Choices.Select(c => new ChoiceInQuestion
            {
                Text = c.Text,
                IsCorrect = c.IsCorrect,
                ImageUrl = c.ImageUrl
            }).ToList()
        };
    }
}