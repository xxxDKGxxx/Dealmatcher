using Dealmatcher.Backend.UseCases.Features.Users.Update;

namespace Dealmatcher.Backend.API.Endpoints.User.Put;

public class PutMe(
    IMediator mediator, 
    IClaimsPrincipalManager claimsPrincipalManager) : 
    Endpoint<PutMeRequest, UserDto>
{
    public override void Configure()
    {
        Version(1);
        Put("/users/me");
           
        Description(d => d
            .Produces<UserDto>(200, "application/json")
            .Produces(400)
            .Produces(401)
            .Produces(500));
           
        Summary(s =>
        {
            s.Summary = "Update current user profile";
            s.Description = "Updates profile information for the authenticated user";
            s.Response<UserDto>(200, "Profile updated successfully");
            s.Response(400, "Invalid update data");
            s.Response(401, "Unauthorized");
            s.Response(500, "Internal server error");
        });
    }

    public override async Task HandleAsync(PutMeRequest req, CancellationToken ct)
    {
        var userId = claimsPrincipalManager.GetUserId(User);

        if (userId is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }
        
        var request = new UpdateUserCommand(userId.Value, req.Name, req.Surname);
        var result = await mediator.Send(request, ct);

        if (result.Status == ResultStatus.NotFound)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }
        
        await result.SendResult(this, ct);
    }
}
