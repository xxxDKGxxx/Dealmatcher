using Dealmatcher.Backend.UseCases.Features.Cart.Delete;

namespace Dealmatcher.Backend.API.Endpoints.Cart.Delete;

public sealed class DeleteItem(IMediator mediator, IClaimsPrincipalManager claimsPrincipalManager)
  : Endpoint<DeleteItemRequest>
{
    public override void Configure()
    {
        Version(1);
        Delete("/cart/items/{CartItemId}");

        Description(d =>
          d.Produces<CartItemDto>(204).Produces(401).Produces(403).Produces(404).Produces(500)
        );

        Summary(s =>
        {
            s.Summary = "Remove item from cart";
            s.Description = "Removes a specific item from the user's cart";
            s.Response<CategoryDto>(204, "Item removed successfully");
            s.Response(401, "Unauthorized");
            s.Response(403, "Forbidden - not your cart item");
            s.Response(404, "Cart item not found");
            s.Response(500, "Internal server error");
        });
    }

    public override async Task HandleAsync(DeleteItemRequest req, CancellationToken ct)
    {
        var requestingUserId = claimsPrincipalManager.GetUserId(User);

        if (requestingUserId is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var request = new DeleteItemFromCartCommand(req.CartItemId, requestingUserId.Value);
        var result = await mediator.Send(request, ct);

        await result.SendResult(this, ct);
    }
}
