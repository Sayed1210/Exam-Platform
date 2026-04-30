namespace Exam.Api;

using System.ComponentModel.DataAnnotations;
using Exam.Models;
using Exam.Service;
using Microsoft.AspNetCore.Mvc;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Auth");

        group.MapPost("/forget-password", ForgetPassword)
            .WithName("ForgetPassword")
            .WithSummary("Request password reset")
            .WithDescription("Sends a password reset link to the user's email if it exists.")
            .Produces<MessageResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest);

        group.MapPost("/reset-password", ResetPassword)
            .WithName("ResetPassword")
            .WithSummary("Reset user password")
            .WithDescription("Resets the user's password using a valid reset token.")
            .Produces<MessageResponse>(StatusCodes.Status200OK)
            .Produces<MessageResponse>(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest);
        
        return app;
    }

    private static async Task<IResult> ForgetPassword(
        [FromBody] ForgetPasswordRequest request,
        [FromServices] IAuthService authService,
        CancellationToken cancellationToken)
    {
        var validationProblem = Validate(request);
        if (validationProblem is not null)
        {
            return validationProblem;
        }

        await authService.ForgetPasswordAsync(request, cancellationToken);
        
        
        return Results.Ok(new MessageResponse("If this email exists, a message has been sent with reset password link."));
    }

    private static async Task<IResult> ResetPassword(
        [FromBody] ResetPasswordRequest request,
        [FromServices] IAuthService authService,
        CancellationToken cancellationToken)
    {
        var validationProblem = Validate(request);
        if (validationProblem is not null)
        {
            return validationProblem;
        }

        var reset = await authService.ResetPasswordAsync(request, cancellationToken);
        if (!reset)
        {
            return Results.BadRequest(new MessageResponse("Invalid or expired reset password token."));
        }

        return Results.Ok(new MessageResponse("Password reset successfully."));
    }

    private static IResult? Validate<TRequest>(TRequest request)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(request!);

        if (Validator.TryValidateObject(request!, context, results, validateAllProperties: true))
        {
            return null;
        }

        var errors = results
            .GroupBy(result => result.MemberNames.FirstOrDefault() ?? string.Empty)
            .ToDictionary(
                group => group.Key,
                group => group.Select(result => result.ErrorMessage ?? "Invalid value.").ToArray());

        return Results.ValidationProblem(errors);
    }
}
