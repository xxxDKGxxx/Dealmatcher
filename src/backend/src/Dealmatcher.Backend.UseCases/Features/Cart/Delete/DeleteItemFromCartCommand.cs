namespace Dealmatcher.Backend.UseCases.Features.Cart.Delete;

public sealed record DeleteItemFromCartCommand(int CartItemId, int UserId) : ICommand<Result>;
