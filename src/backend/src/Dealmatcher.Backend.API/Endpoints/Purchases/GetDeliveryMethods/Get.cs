namespace Dealmatcher.Backend.API.Endpoints.Purchases.GetDeliveryMethods;

public class Get(IMediator mediator) : EndpointWithoutRequest<List<DeliveryMethodDto>>
{
    public override void Configure()
    {
        Version(1);
        Get("/purchases/delivery-methods");
        AllowAnonymous();

        Description(d =>
            d.Produces<List<DeliveryMethodDto>>(200, "application/json").Produces(500)
        );

        Summary(s =>
        {
            s.Summary = "Get available delivery methods";
            s.Description = "Returns all available delivery options";
            s.Response<List<DeliveryMethodDto>>(200, "Delivery methods retrieved successfully");
            s.Response(500, "Internal server error");
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var request = new GetDeliveryMethodsQuery();
        var result = await mediator.Send(request, ct);

        await result.SendResult(this, ct);
    }
}
