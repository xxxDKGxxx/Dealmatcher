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
            s.Response(200, "Processed");
            s.Response(400, "Invalid payload");
            s.Response(404, "Purchase not found");
            s.Response(409, "Concurrency conflict");
            s.Response(500, "Internal server error");
        });
    }

    public override async Task HandleAsync(ProcessPaymentRequest request, CancellationToken ct)
    {
        using var reader = new StreamReader(HttpContext.Request.Body);
        var rawBody = await reader.ReadToEndAsync(ct);

        var command = new ProcessPaymentCommand(request.SessionId, rawBody);
        var result = await mediator.Send(command, ct);
        await result.SendResult(this, ct);
    }
}
