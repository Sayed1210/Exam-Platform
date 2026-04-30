using Exam.Repo;
using Microsoft.Extensions.DependencyInjection;

namespace Exam.Repo;

public static class DependencyInjection
{
    public static IServiceCollection AddRepositoryLayer(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();


        return services;
    }
}
