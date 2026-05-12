namespace Dealmatcher.Backend.UseCases.Features.Cart.GetTotal;

public sealed record GetCartTotalQuery(int UserId) : IQuery<Result<CartTotalDto>>;
