using Dealmatcher.Backend.UseCases.Features.Cart.GetTotal;

namespace Dealmatcher.Backend.API.Endpoints.Cart.GetTotal;

public class GetCartTotal(
    IMediator mediator,
    IClaimsPrincipalManager claimsPrincipalManager)
    : EndpointWithoutRequest<CartTotalDto>
{
    public override void Configure()
    {
        Version(1);
        Get("cart/total");

        Description(d => d.Produces<CartTotalDto>(200)
                            .Produces(401)
                            .Produces(500));

        Summary(s =>
        {
            s.Summary = "Get cart total";
            s.Description = "Returns the total price of all items in the cart";

            s.Response<CartTotalDto>(200, "Cart total retrieved successfully", "application/json");
            s.Response(401, "Unauthorized");
            s.Response(500, "Internal server error");
        });
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var userId = claimsPrincipalManager.GetUserId(User);

        if (userId is null)
        {
            await SendUnauthorizedAsync(cancellationToken);
            return;
        }

        var getCartTotalQuery = new GetCartTotalQuery(userId.Value);
        var result = await mediator.Send(getCartTotalQuery, cancellationToken);

        await result.SendResult(this, cancellationToken);
    }
}
