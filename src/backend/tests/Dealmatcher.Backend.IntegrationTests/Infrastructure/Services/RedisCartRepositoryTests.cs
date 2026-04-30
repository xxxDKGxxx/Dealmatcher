using Dealmatcher.Backend.Domain.Core.Cart;
using Dealmatcher.Backend.Infrastructure.Services.CartRepositories;
using StackExchange.Redis;
using Testcontainers.Redis;

namespace Dealmatcher.Backend.IntegrationTests.Infrastructure.Services;

public class RedisCartRepositoryTests : IAsyncLifetime
{
    private readonly RedisContainer _redisContainer = new RedisBuilder("redis:7-alpine").Build();

    private RedisCartRepository _repository = null!;
    private ConnectionMultiplexer? _redis = null!;

    public async Task InitializeAsync()
    {
        await _redisContainer.StartAsync();
        _redis = await ConnectionMultiplexer.ConnectAsync(_redisContainer.GetConnectionString());
        _repository = new RedisCartRepository(_redis);
    }

    public async Task DisposeAsync()
    {
        _redis!.Dispose();
        await _redisContainer.DisposeAsync();
    }

    [Fact]
    public async Task GetCartAsync_NoCart_ReturnsEmptyCart()
    {
        var cart = await _repository.GetCartAsync(1, CancellationToken.None);

        cart.UserId.ShouldBe(1);
        cart.Items.Count.ShouldBe(0);
    }

    [Fact]
    public async Task SaveAndGetCart_SingleItem_RoundTrips()
    {
        var cart = new Cart(1);
        cart.UpdateItemQuantity(10, 2);

        await _repository.SaveCartAsync(cart, CancellationToken.None);
        var loaded = await _repository.GetCartAsync(1, CancellationToken.None);

        loaded.UserId.ShouldBe(1);
        loaded.Items.Count.ShouldBe(1);
        loaded.Items.First().OfferId.ShouldBe(10);
        loaded.Items.First().Quantity.ShouldBe(2);
    }

    [Fact]
    public async Task SaveAndGetCart_MultipleItems_RoundTrips()
    {
        var cart = new Cart(2);
        cart.UpdateItemQuantity(10, 1);
        cart.UpdateItemQuantity(20, 3);
        cart.UpdateItemQuantity(30, 5);

        await _repository.SaveCartAsync(cart, CancellationToken.None);
        var loaded = await _repository.GetCartAsync(2, CancellationToken.None);

        loaded.Items.Count.ShouldBe(3);
    }

    [Fact]
    public async Task SaveCart_OverwritesExisting()
    {
        var cart = new Cart(3);
        cart.UpdateItemQuantity(10, 1);
        await _repository.SaveCartAsync(cart, CancellationToken.None);

        var cart2 = new Cart(3);
        cart2.UpdateItemQuantity(20, 5);
        await _repository.SaveCartAsync(cart2, CancellationToken.None);

        var loaded = await _repository.GetCartAsync(3, CancellationToken.None);

        loaded.Items.Count.ShouldBe(1);
        loaded.Items.First().OfferId.ShouldBe(20);
    }

    [Fact]
    public async Task GetCart_DifferentUsers_Isolated()
    {
        var cart1 = new Cart(1);
        cart1.UpdateItemQuantity(10, 1);
        await _repository.SaveCartAsync(cart1, CancellationToken.None);

        var cart2 = new Cart(2);
        cart2.UpdateItemQuantity(20, 3);
        await _repository.SaveCartAsync(cart2, CancellationToken.None);

        var loaded1 = await _repository.GetCartAsync(1, CancellationToken.None);
        var loaded2 = await _repository.GetCartAsync(2, CancellationToken.None);

        loaded1.Items.Count.ShouldBe(1);
        loaded1.Items.First().OfferId.ShouldBe(10);
        loaded2.Items.Count.ShouldBe(1);
        loaded2.Items.First().OfferId.ShouldBe(20);
    }

    [Fact]
    public async Task SaveCart_UpdateQuantity_Persists()
    {
        var cart = new Cart(4);
        cart.UpdateItemQuantity(10, 1);
        await _repository.SaveCartAsync(cart, CancellationToken.None);

        var loaded = await _repository.GetCartAsync(4, CancellationToken.None);
        loaded.UpdateItemQuantity(10, 5);
        await _repository.SaveCartAsync(loaded, CancellationToken.None);

        var reloaded = await _repository.GetCartAsync(4, CancellationToken.None);
        reloaded.Items.Count.ShouldBe(1);
        reloaded.Items.First().Quantity.ShouldBe(5);
    }

    [Fact]
    public async Task SaveCart_RemoveItem_Persists()
    {
        var cart = new Cart(5);
        cart.UpdateItemQuantity(10, 1);
        cart.UpdateItemQuantity(20, 2);
        await _repository.SaveCartAsync(cart, CancellationToken.None);

        var loaded = await _repository.GetCartAsync(5, CancellationToken.None);
        loaded.RemoveItem(10);
        await _repository.SaveCartAsync(loaded, CancellationToken.None);

        var reloaded = await _repository.GetCartAsync(5, CancellationToken.None);
        reloaded.Items.Count.ShouldBe(1);
        reloaded.Items.First().OfferId.ShouldBe(20);
    }

    [Fact]
    public async Task SaveCart_ClearCart_Persists()
    {
        var cart = new Cart(6);
        cart.UpdateItemQuantity(10, 1);
        cart.UpdateItemQuantity(20, 2);
        await _repository.SaveCartAsync(cart, CancellationToken.None);

        var loaded = await _repository.GetCartAsync(6, CancellationToken.None);
        loaded.Clear();
        await _repository.SaveCartAsync(loaded, CancellationToken.None);

        var reloaded = await _repository.GetCartAsync(6, CancellationToken.None);
        reloaded.Items.Count.ShouldBe(0);
    }
}
