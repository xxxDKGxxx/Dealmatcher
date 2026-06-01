using Dealmatcher.Backend.UseCases.Features.Activities.GetOfferActivities;

namespace Dealmatcher.Backend.API.Endpoints.Activities.GetOfferActivity;

public sealed class GetOfferActivityEndpoint(
    IMediator mediator,
    IClaimsPrincipalManager claimsManager)
    : Endpoint<GetOfferActivityRequest, List<ActivityDto>>
{
    public override void Configure()
    {
        Version(1);
        Get("/admin/activity/offer/{OfferId}");
        Roles("Admin");
        Description(d => d
            .Produces<List<ActivityDto>>(200, "application/json")
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .Produces(500));
        Summary(s =>
        {
            s.Summary = "Get offer activity (admin)";
            s.Description = "Returns activity history for a specific offer";
            s.Response<List<ActivityDto>>(200, "Offer activity retrieved successfully");
            s.Response(401, "Unauthorized");
            s.Response(403, "Forbidden - admin only");
            s.Response(404, "Offer not found");
            s.Response(500, "Internal server error");
        });
    }

    public override async Task HandleAsync(GetOfferActivityRequest request, CancellationToken ct)
    {
        var adminId = claimsManager.GetUserId(User);
        if (adminId is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var query = new GetOfferActivityQuery(adminId.Value, request.OfferId, request.From, request.To);
        var result = await mediator.Send(query, ct);
        await result.SendResult(this, ct);
    }
}
