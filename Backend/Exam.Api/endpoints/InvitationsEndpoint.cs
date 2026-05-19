using Exam.Models;
using Exam.Service;             
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Exam.Api;

public static class InvitationsEndpoint
{
    public static void MapInvitationsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/invitations");

        group.MapPost("/send", async (SendInvitationRequest request, IInvitationValidator validator,IInvitationService service) =>
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Results.BadRequest(new InvitationStatusResponse(
                    false, 
                    validationResult.Message, 
                    DateTime.UtcNow
                ));
            }
            var response = await service.SendInvitationAsync(request);
            
            return response.IsSuccess 
                ? Results.Ok(response) 
                : Results.BadRequest(response); 
        });
    }
}