namespace Dealmatcher.Backend.API.Endpoints.Cart.Update;

public class UpdateItem(
    IMediator mediator,
    IClaimsPrincipalManager claimsPrincipalManager)
    : Endpoint<UpdateItemRequest, CartItemDto>
{
    public override void Configure()
    {
        Version(1);
        Patch("/cart/items/{CartItemId}");

        Description(d => d
            .Produces<CartItemDto>(200, "application/json")
            .Produces(400)
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .Produces(500));

        Summary(s =>
        {
            s.Summary = "Update cart item quantity";
            s.Description = "Changes the quantity of a specific cart item";
            s.Response<CartItemDto>(200, "Quantity updated successfully");
            s.Response(400, "Invalid quantity");
            s.Response(401, "Unauthorized");
            s.Response(403, "Forbidden - not your cart item");
            s.Response(404, "Cart item not found");
            s.Response(500, "Internal server error");
        });
    }

    public override async Task HandleAsync(UpdateItemRequest req, CancellationToken ct)
    {
        var requestingUserId = claimsPrincipalManager.GetUserId(User);

        if (requestingUserId is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var request = new UpdateCartItemQuantityCommand(requestingUserId.Value, req.CartItemId, req.Quantity);
        var result = await mediator.Send(request, ct);

        await result.SendResult(this, ct);
    }
}
