using Exam.Service;

public static class VerifyLinkEndpoints
{
    public static void MapVerifyLinkEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/verify-invitation", async (
            string token,
            IVerifyInvitationService service) =>
        {
            var result = await service.VerifyInvitation(token);

            if (result == null)
                return Results.NotFound("Invalid or expired token");

            return Results.Ok(result);
        });
    }
}