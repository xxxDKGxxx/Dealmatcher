using Dealmatcher.Backend.UseCases.Features.Offers.ChangeStatus;

namespace Dealmatcher.Backend.API.Endpoints.Offer.ChangeStatus;

public sealed class ChangeStatus(
    IMediator mediator,
    IClaimsPrincipalManager claimsManager) : Endpoint<ChangeStatusRequest, OfferDto>
{
    public override void Configure()
    {
        Version(1);
        Put("/offers/{OfferId}/status");

        Description(d => d
            .Produces<OfferDto>(200, "application/json")
            .Produces(400)
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .Produces(409)
            .Produces(500));

        Summary(s =>
        {
            s.Summary = "Update offer status";
            s.Description = "Changes offer status (ACTIVE/PROMOTED/SOLD) - permissions apply.";
            s.Response<OfferDto>(200, "Status updated successfully");
            s.Response(400, "Invalid status value");
            s.Response(401, "Unauthorized");
            s.Response(403, "Forbidden - insufficient permissions");
            s.Response(404, "Offer not found");
            s.Response(409, "Cannot change status from current state");
            s.Response(500, "Internal server error");
        });
    }

    public override async Task HandleAsync(ChangeStatusRequest req, CancellationToken ct)
    {
        var userId = claimsManager.GetUserId(User);
        if (userId is null)
        {
            await SendUnauthorizedAsync(cancellation: ct);
            return;
        }

        Result<OfferDto> result;

        switch (req.Status.ToUpperInvariant())
        {
            case "ACTIVE":
                var activateCommand = new ActivateOfferCommand(userId.Value, req.OfferId);
                result = await mediator.Send(activateCommand, ct);
                break;

            case "SOLD":
                var soldCommand = new SetOfferSoldCommand(userId.Value, req.OfferId);
                result = await mediator.Send(soldCommand, ct);
                break;

            default:
                AddError(r => r.Status, $"Invalid status: '{req.Status}'. Allowed values: ACTIVE, SOLD.");
                await SendErrorsAsync(cancellation: ct);
                return;
        }

        await result.SendResult(this, ct);
    }
}
