namespace Dealmatcher.Backend.API.Endpoints.Offer.Get;

public sealed class GetById(
    IClaimsPrincipalManager claimsManager,
    IMediator mediator) : Endpoint<GetByIdRequest, OfferDto>
{
    public override void Configure()
    {
        Version(1);
        AllowAnonymous();
        Get("/offers/{OfferId}");

        Description(d => d.Produces<OfferDto>(200, "application/json").Produces(404).Produces(500));

        Summary(s =>
        {
            s.Summary = "Get offer details";
            s.Description = "Returns detailed information about a specific offer";
            s.Response<UserDto>(200, "Offer details retrieved successfully");
            s.Response(404, "Offer not found");
            s.Response(500, "Internal server error");
        });
    }

    public override async Task HandleAsync(GetByIdRequest req, CancellationToken ct)
    {
        var userId = claimsManager.GetUserId(User);
        var request = new GetOfferQuery(req.OfferId, userId);
        var result = await mediator.Send(request, ct);

        await result.SendResult(this, ct);
    }
}
