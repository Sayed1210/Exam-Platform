using Exam.Repo;
using Microsoft.Extensions.DependencyInjection;

namespace Exam.Repo;

public static class DependencyInjection
{
    public static IServiceCollection AddRepositoryLayer(this IServiceCollection services)
    {
      services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
        services.AddScoped<ICandidateRepository, CandidateRepository>();
        services.AddScoped<ICandidateAnswerRepository, CandidateAnswerRepository>();
        services.AddScoped<ICandidateExamRepository, CandidateExamRepository>();
        services.AddScoped<IExamRepository, ExamRepository>();
        services.AddScoped<IQuestionRepository, QuestionRepository>();
        services.AddScoped<ITopicRepository, TopicRepository>();

        return services;
    }
}
