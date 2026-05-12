namespace Dealmatcher.Backend.Domain.Interfaces.CartRepository;

public interface ICartRepository
{
    Task<Cart> GetCartAsync(int userId, CancellationToken ct);
    Task SaveCartAsync(Cart cart, CancellationToken ct);
}
