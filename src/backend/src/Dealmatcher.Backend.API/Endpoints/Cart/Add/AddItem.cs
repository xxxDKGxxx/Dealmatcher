namespace Dealmatcher.Backend.API.Endpoints.Cart.Add;

public class AddItem(IMediator mediator, IClaimsPrincipalManager claimsPrincipalManager)
  : Endpoint<AddItemRequest, CartItemDto>
{
    public override void Configure()
    {
        Version(1);
        Post("/cart/items");
        AllowAnonymous();

        Description(d =>
          d.Produces<CartItemDto>(201, "application/json")
            .Produces(400)
            .Produces(401)
            .Produces(404)
            .Produces(409)
            .Produces(500)
        );

        Summary(s =>
        {
            s.Summary = "Add item to cart";
            s.Description = "Adds na offer to the user's cart";
            s.Response<CategoryDto>(201, "Item added to cart successfully");
            s.Response(400, "Invalid request");
            s.Response(401, "Unauthorized");
            s.Response(404, "Offer not found");
            s.Response(409, "Item already in cart");
            s.Response(500, "Internal server error");
        });
    }

    public override async Task HandleAsync(AddItemRequest req, CancellationToken ct)
    {
        var requestingUserId = claimsPrincipalManager.GetUserId(User);

        if (requestingUserId is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var request = new AddItemToCartCommand(requestingUserId.Value, req.OfferId, req.Quantity);
        var result = await mediator.Send(request, ct);

        await result.SendResult(this, ct);
    }
}
