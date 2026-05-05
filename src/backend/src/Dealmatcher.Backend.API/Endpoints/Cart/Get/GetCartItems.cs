namespace Dealmatcher.Backend.API.Endpoints.Cart.Get;

public class GetCartItems(
    IMediator mediator,
    IClaimsPrincipalManager claimsPrincipalManager)
    : EndpointWithoutRequest<List<CartItemDto>>
{
    public override void Configure()
    {
        Version(1);
        Get("cart/items");

        Description(d => d.Produces<List<CartItemDto>>(200, "application/json")
                            .Produces(401)
                            .Produces(500)
        );

        Summary(s =>
        {
            s.Summary = "Get cart contents";
            s.Description = "Returns all items in the user's cart";

            s.Response<List<CartItemDto>>(200, "Cart items retrieved successfully");
            s.Response(401, "Unauthorized");
            s.Response(500, "Internal server error");
        });
    }

    public override async Task HandleAsync(CancellationToken cancellation)
    {
        var userId = claimsPrincipalManager.GetUserId(User);

        if (userId is null)
        {
            await SendUnauthorizedAsync(cancellation);
            return;
        }

        var query = new GetCartItemsQuery(userId.Value);
        var result = await mediator.Send(query, cancellation);

        await result.SendResult(this, cancellation);
    }
}
