using Dealmatcher.Backend.UseCases.Features.Purchases.GetPaymentMethods;

namespace Dealmatcher.Backend.API.Endpoints.Purchases.GetPaymentMethods;

public sealed class GetPaymentMethodsEndpoint(IMediator mediator) : EndpointWithoutRequest<List<PaymentMethodDto>>
{
    public override void Configure()
    {
        Version(1);
        AllowAnonymous();
        Get("/purchases/payment-methods");

        Description(d => d.Produces<List<PaymentMethodDto>>(200, "application/json").Produces(500));

        Summary(s =>
        {
            s.Summary = "Get available payment methods";
            s.Description = "Returns all available payment options";
            s.Response<List<PaymentMethodDto>>(200, "Payment methods retrieved successfully");
            s.Response(500, "Internal server error");
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var request = new GetPaymentMethodsQuery();
        var result = await mediator.Send(request, ct);

        await result.SendResult(this, ct);
    }
}
