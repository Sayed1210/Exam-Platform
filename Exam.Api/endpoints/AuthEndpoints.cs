using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Exam.Models;
using Exam.Service;


public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", Login)
            .AllowAnonymous()
            .WithName("Login")
            .WithTags("Auth");

        return app;
    }

    private static async Task<IResult> Login(
        [FromBody] LoginRequest request,
        [FromServices] IAuthService authService,
        CancellationToken cancellationToken)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(request);

        var isValid = Validator.TryValidateObject(
            request,
            validationContext,
            validationResults,
            validateAllProperties: true);

        if (!isValid)
        {
            var errors = validationResults
                .GroupBy(result => result.MemberNames.FirstOrDefault() ?? string.Empty)
                .ToDictionary(
                    group => group.Key,
                    group => group
                        .Select(result => result.ErrorMessage ?? "Invalid value")
                        .ToArray());

            return Results.ValidationProblem(errors);
        }

        var result = await authService.LoginAsync(request, cancellationToken);

        if (result is null)
        {
            return Results.Unauthorized();
        }

        return Results.Ok(result);
    }
}
