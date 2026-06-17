namespace Dealmatcher.Backend.API.Endpoints.Purchases.ProcessPayment;

public sealed class ProcessPaymentEndpoint(IMediator mediator)
    : Endpoint<ProcessPaymentRequest>
{
    public override void Configure()
    {
        Version(1);
        AllowAnonymous();
        Post("/purchases/webhook/{SessionId}");
        Description(d => d
            .Produces(200)
            .Produces(400)
            .Produces(404)
            .Produces(409)
            .Produces(500));
        Summary(s =>
        {
            s.Summary = "Payment webhook";
            s.Description = "Called by payment provider to notify about payment result";
        });
    }

    public override async Task HandleAsync(ProcessPaymentRequest request, CancellationToken ct)
    {
        var command = new ProcessPaymentCommand(request.SessionId, request.ProviderStatus);
        var result = await mediator.Send(command, ct);
        await result.SendResult(this, ct);
    }
}
