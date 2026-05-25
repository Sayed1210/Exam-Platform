using Exam.Api.Validation;
using Exam.Models;
using Exam.Service;

namespace Exam.Api;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users")
            .WithTags("Users")
            .RequireAuthorization(policy => policy.RequireRole(UserRole.Owner.ToString()));

        group.MapGet("", async (
            IUserManagementService service,
            CancellationToken cancellationToken) =>
        {
            var users = await service.GetUsersAsync(cancellationToken);
            return Results.Ok(users);
        });

        group.MapPost("", async (
            CreateUserRequest request,
            IUserManagementService service,
            CancellationToken cancellationToken) =>
        {
            var validationProblem = EndpointValidation.Request(request);
            if (validationProblem is not null)
                return validationProblem;

            var (user, error) = await service.CreateAdminAsync(request, cancellationToken);

            return user is not null
                ? Results.Created($"/api/users/{user.Id}", user)
                : Results.BadRequest(new { message = error });
        });

        return app;
    }
}
