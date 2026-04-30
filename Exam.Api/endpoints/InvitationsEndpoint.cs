using Exam.Models.Dtos.Requests;
using Exam.Service;             
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;



public static class InvitationsEndpoint
{
    public static void MapInvitationsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/invitations");

        group.MapPost("/send", async (SendInvitationRequest request, IInvitationService service) =>
        {
            var response = await service.SendInvitationAsync(request);
            
            return response.IsSuccess 
                ? Results.Ok(response) 
                : Results.BadRequest(response); 
        });
    }
}