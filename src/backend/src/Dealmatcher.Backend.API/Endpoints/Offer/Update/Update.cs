
namespace Dealmatcher.Backend.API.Endpoints.Offer.Update;

public sealed class Update(
    IMediator mediator,
    IClaimsPrincipalManager claimsManager)
    : Endpoint<UpdateRequest, OfferDto>
{
    public override void Configure()
    {
        Version(1);
        Patch("/offers/{OfferId}");

        Description(d => d
            .Produces<OfferDto>(200, "application/json")
            .Produces(400)
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .Produces(500));

        Summary(s =>
        {
            s.Summary = "Update an offer";
            s.Description = "Updates an existing offer (returns to DRAFT status for revalidation)";
            s.Response<OfferDto>(200, "Offer updated successfully");
            s.Response(400, "Invalid update data");
            s.Response(401, "Unauthorized");
            s.Response(403, "Forbidden - not the offer owner");
            s.Response(404, "Offer not found");
            s.Response(500, "Internal server error");
        });
    }

    public override async Task HandleAsync(UpdateRequest request, CancellationToken cancellationToken)
    {
        var userId = claimsManager.GetUserId(User);
        if (userId is null)
        {
            await SendUnauthorizedAsync(cancellation: cancellationToken);
            return;
        }

        var command = new UpdateOfferCommand(
            request.OfferId,
            userId.Value,
            request.Title,
            request.Description,
            request.Price,
            request.Images,
            request.Tags,
            request.Properties,
            request.Availability);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            await SendAsync(result.Value, statusCode: 200, cancellation: cancellationToken);
            return;
        }

        await result.SendResult(this, ct: cancellationToken);
    }
}
