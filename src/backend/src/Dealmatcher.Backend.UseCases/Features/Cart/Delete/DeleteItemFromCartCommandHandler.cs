namespace Dealmatcher.Backend.UseCases.Features.Cart.Delete;

public sealed class DeleteItemFromCartCommandHandler(ICartRepository cartRepository)
  : ICommandHandler<DeleteItemFromCartCommand, Result>
{
    public async Task<Result> Handle(
      DeleteItemFromCartCommand request,
      CancellationToken cancellationToken
    )
    {
        var cart = await cartRepository.GetCartAsync(request.UserId, cancellationToken);

        if (!cart.Items.Where(ci => ci.OfferId == request.CartItemId).Any())
        {
            return Result.NotFound("Cart item not found");
        }

        cart.RemoveItem(request.CartItemId);

        await cartRepository.SaveCartAsync(cart, cancellationToken);

        return Result.NoContent();
    }
}
