using Dealmatcher.Backend.UseCases.Features.Offers.List;

namespace Dealmatcher.Backend.API.Endpoints.User.Offers.Get;

public class GetMeOffers(
    IMediator mediator,
    IClaimsPrincipalManager claimsPrincipalManager) : EndpointWithoutRequest<List<OfferDto>>
{
    public override void Configure()
    {
        Version(1);
        Get("/users/me/offers");

        Description(d => d
            .Produces<List<OfferDto>>(200, "application/json")
            .Produces(204)
            .Produces(401)
            .Produces(500));

        Summary(s =>
        {
            s.Summary = "Get my offers";
            s.Description = "Returns all offers posted by the authenticated user";
            s.Response<List<OfferDto>>(200, "Offers retrieved successfully");
            s.Response(204, "No offers found");
            s.Response(401, "Unauthorized");
            s.Response(500, "Internal server error");
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = claimsPrincipalManager.GetUserId(User);

        if (userId == null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var query = new ListOffersByUserIdQuery(userId.Value);
        var result = await mediator.Send(query, ct);

        if (result.Status == ResultStatus.NotFound)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        await result.SendResult(this, ct);
    }
}
