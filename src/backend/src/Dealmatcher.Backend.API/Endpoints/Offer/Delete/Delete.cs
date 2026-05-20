
namespace Dealmatcher.Backend.API.Endpoints.Offer.Delete;

public sealed class Delete(
    IMediator mediator,
    IClaimsPrincipalManager claimsManager)
    : Endpoint<DeleteRequest>
{
    public override void Configure()
    {
        Version(1);
        Delete("/offers/{OfferId}");

        Description(d => d
            .Produces(204)
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .Produces(500));

        Summary(s =>
        {
            s.Summary = "Delete an offer";
            s.Description = "Soft-deletes an offer (seller or admin only)";
            s.Response(204, "Offer deleted successfully");
            s.Response(401, "Unauthorized");
            s.Response(403, "Forbidden - not the offer owner or admin");
            s.Response(404, "Offer not found");
            s.Response(500, "Internal server error");
        });
    }

    public override async Task HandleAsync(DeleteRequest request, CancellationToken cancellationToken)
    {
        var userId = claimsManager.GetUserId(User);

        if (userId is null)
        {
            await SendUnauthorizedAsync(cancellation: cancellationToken);
            return;
        }

        var command = new DeleteOfferCommand(request.OfferId, userId.Value);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            await SendNoContentAsync(cancellation: cancellationToken);
            return;
        }

        await result.SendResult(this, ct: cancellationToken);
    }
}
