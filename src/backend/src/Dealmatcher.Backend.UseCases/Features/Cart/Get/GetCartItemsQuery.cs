namespace Dealmatcher.Backend.UseCases.Features.Cart.Get;

public sealed record GetCartItemsQuery(int UserId) : IQuery<Result<List<CartItemDto>>>;
