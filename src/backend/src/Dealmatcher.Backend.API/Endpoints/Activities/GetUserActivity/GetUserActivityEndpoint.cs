using Dealmatcher.Backend.UseCases.Features.Activities.GetUserActivity;

namespace Dealmatcher.Backend.API.Endpoints.Activities.GetUserActivity;

public sealed class GetUserActivityEndpoint(
    IMediator mediator,
    IClaimsPrincipalManager claimsManager)
    : Endpoint<GetUserActivityRequest, List<ActivityDto>>
{
    public override void Configure()
    {
        Version(1);
        Get("/admin/activity/user/{UserId}");
        Roles("Admin");
        Description(d => d
            .Produces<List<ActivityDto>>(200, "application/json")
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .Produces(500));
        Summary(s =>
        {
            s.Summary = "Get user activity (admin)";
            s.Description = "Returns activity history for a specific user";
            s.Response<List<ActivityDto>>(200, "User activity retrieved successfully");
            s.Response(401, "Unauthorized");
            s.Response(403, "Forbidden - admin only");
            s.Response(404, "User not found");
            s.Response(500, "Internal server error");
        });
    }

    public override async Task HandleAsync(GetUserActivityRequest request, CancellationToken ct)
    {
        var adminId = claimsManager.GetUserId(User);
        if (adminId is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var query = new GetUserActivityQuery(adminId.Value, request.UserId, request.From, request.To);
        var result = await mediator.Send(query, ct);
        await result.SendResult(this, ct);
    }
}
