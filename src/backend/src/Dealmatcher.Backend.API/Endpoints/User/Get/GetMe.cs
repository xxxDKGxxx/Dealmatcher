using Dealmatcher.Backend.UseCases.Features.Users.Get;

namespace Dealmatcher.Backend.API.Endpoints.User.Get;

public class GetMe(
    IMediator mediator,
    IClaimsPrincipalManager claimsPrincipalManager) :
    EndpointWithoutRequest<UserDto>
{
    public override void Configure()
    {
        Version(1);
                   Get("/users/me");
           
                   Description(d => d
                       .Produces<UserDto>(200, "application/json")
                       .Produces(401)
                       .Produces(500));
           
                   Summary(s =>
                   {
                       s.Summary = "Get current user profile";
                       s.Description = "Returns profile information for the authenticated user";
                       s.Response<UserDto>(200, "Profile retrieved successfully");
                       s.Response(401, "Unauthorized");
                       s.Response(500, "Internal server error");
                   });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = claimsPrincipalManager.GetUserId(User);

        if (userId is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var query = new GetUserProfileQuery(userId.Value);
        var result = await mediator.Send(query, ct);

        if (result.Status == ResultStatus.NotFound)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        await result.SendResult(this, ct);
    }
}
