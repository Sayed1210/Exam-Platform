using Exam.Repo.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Exam.Repo;

public static class DependencyInjection
{
    public static IServiceCollection AddRepositoryLayer(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
