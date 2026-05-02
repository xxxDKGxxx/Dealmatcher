namespace Dealmatcher.Backend.UseCases.Features.Cart.Update;

public sealed record UpdateCartItemQuantityCommand(
    int UserId,
    int OfferId,
    int Quantity) : ICommand<Result<CartItemDto>>;
