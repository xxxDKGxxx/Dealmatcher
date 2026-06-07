using Dealmatcher.Backend.UseCases.Features.Purchases.Initialize;

namespace Dealmatcher.Backend.API.Endpoints.Purchases.Initialize;

public sealed class InitializePurchaseEndpoint(
    IMediator mediator,
    IClaimsPrincipalManager claimsManager)
    : Endpoint<InitializePurchaseRequest, InitializePurchaseResult>
{
    public override void Configure()
    {
        Version(1);
        Post("/purchases/initialize");

        Description(d => d
            .Produces<InitializePurchaseResult>(200, "application/json")
            .Produces(400)
            .Produces(401)
            .Produces(404)
            .Produces(409)
            .Produces(500));

        Summary(s =>
        {
            s.Summary = "Initialize purchase";
            s.Description = "Creates an order and redirects to external payment provider";
            s.Response<InitializePurchaseResult>(200, "Purchase initialized successfully");
            s.Response(400, "Invalid purchase data");
            s.Response(401, "Unauthorized");
            s.Response(404, "Offer not found");
            s.Response(409, "Offer not available for purchase");
            s.Response(500, "Internal server error");
        });
    }

    public override async Task HandleAsync(InitializePurchaseRequest request, CancellationToken cancellationToken)
    {
        var userId = claimsManager.GetUserId(User);
        if (userId is null)
        {
            await SendUnauthorizedAsync(cancellation: cancellationToken);
            return;
        }

        var command = new InitializePurchaseCommand(
            userId.Value,
            request.OfferId,
            request.DeliveryMethodId,
            request.PaymentMethodId,
            request.Quantity);

        var result = await mediator.Send(command, cancellationToken);

        await result.SendResult(this, ct: cancellationToken);
    }
}
