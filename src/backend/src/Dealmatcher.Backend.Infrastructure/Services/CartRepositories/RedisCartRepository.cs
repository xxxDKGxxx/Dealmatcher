using System.Text.Json;
using Dealmatcher.Backend.Domain.Core.Cart;
using StackExchange.Redis;

namespace Dealmatcher.Backend.Infrastructure.Services.CartRepositories;

public sealed class RedisCartRepository(IConnectionMultiplexer redis) : ICartRepository
{
    private readonly IDatabase _db = redis.GetDatabase();
    private static readonly TimeSpan _expiry = TimeSpan.FromDays(7);
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private sealed record CartItemData(int OfferId, int Quantity);
    private sealed record CartData(List<CartItemData> Items);

    private static string Key(int userId) => $"cart:{userId}";

    public async Task<Cart> GetCartAsync(int userId, CancellationToken ct)
    {
        var json = await _db.StringGetAsync(Key(userId));
        if (json.IsNullOrEmpty)
            return new Cart(userId);

        var data = JsonSerializer.Deserialize<CartData>(json!, _jsonOptions)!;
        var cart = new Cart(userId);
        foreach (var item in data.Items)
            cart.UpdateItemQuantity(item.OfferId, item.Quantity);

        return cart;
    }

    public async Task SaveCartAsync(Cart cart, CancellationToken ct)
    {
        var data = new CartData([.. cart.Items.Select(i => new CartItemData(i.OfferId, i.Quantity))]);
        var json = JsonSerializer.Serialize(data, _jsonOptions);
        await _db.StringSetAsync(Key(cart.UserId), json, _expiry);
    }
}
