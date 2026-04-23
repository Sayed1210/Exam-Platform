using Microsoft.AspNetCore.Mvc;
public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var auth = app.MapGroup("/api/auth");

        // POST: /api/auth/login
        auth.MapPost("/login", async (
            LoginRequest request,
            [FromServices] IAuthService authService) =>
        {
            var result = await authService.LoginAsync(request);

            if (result is null)
                return Results.Json(
                    new { message = "Incorrect email or password" },
                        statusCode: StatusCodes.Status401Unauthorized
    );

            return Results.Ok(result);
        });
    }
}