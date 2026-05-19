using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Exam.Models;
using Exam.Service;
using Exam.Api.Validation;
namespace Exam.Api;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {

        var group = app.MapGroup("/api/auth")
            .WithTags("Auth");

        group.MapPost("/login", Login)
            .AllowAnonymous()
            .WithName("Login")
            .WithSummary("Login")
            .WithDescription("Logs user in and returns jwt token.")
            .Produces<MessageResponse>(StatusCodes.Status200OK)
            .Produces<MessageResponse>(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest);
        
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

        group.MapPost("/logout", (HttpContext httpContext) =>
        {
            httpContext.Response.Cookies.Delete("token");
            return Results.Ok();
        });

        return app;
    }

    private static async Task<IResult> Login(
        [FromBody] LoginRequest request,
        [FromServices] IAuthService authService,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var validationProblem = EndpointValidation.Request(request);
        if (validationProblem is not null)
        {
            return validationProblem;
        }

        var result = await authService.LoginAsync(request, cancellationToken);

        if (result is null)
        {
            return Results.Unauthorized();
        }
        // SET COOKIE HERE
        httpContext.Response.Cookies.Append("token", result.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // change to true in production
            SameSite = SameSiteMode.Lax,
            Expires = result.ExpiresAt
        });
    
        return Results.Ok(result);
    }


    private static async Task<IResult> ForgetPassword(
        [FromBody] ForgetPasswordRequest request,
        [FromServices] IAuthService authService,
        CancellationToken cancellationToken)
    {
       var validationProblem = EndpointValidation.Request(request);
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
        var validationProblem = EndpointValidation.Request(request);
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
}
