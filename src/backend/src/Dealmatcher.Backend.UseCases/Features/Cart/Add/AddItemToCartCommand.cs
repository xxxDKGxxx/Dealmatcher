namespace Dealmatcher.Backend.UseCases.Features.Cart.Add;

public sealed record AddItemToCartCommand(int UserId, int OfferId, int Quantity)
  : ICommand<Result<CartItemDto>>;
