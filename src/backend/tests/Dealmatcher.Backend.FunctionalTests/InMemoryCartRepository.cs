namespace Dealmatcher.Backend.FunctionalTests;

public class InMemoryCartRepository : ICartRepository
{
    private readonly ConcurrentDictionary<int, Cart> _carts = new();

    public Task<Cart> GetCartAsync(int userId, CancellationToken ct)
    {
        var cart = _carts.GetOrAdd(userId, id => new Cart(id));
        return Task.FromResult(cart);
    }

    public Task SaveCartAsync(Cart cart, CancellationToken ct)
    {
        _carts[cart.UserId] = cart;
        return Task.CompletedTask;
    }

    public void ClearAll()
    {
        _carts.Clear();
    }
}
