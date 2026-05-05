namespace Dealmatcher.Backend.FunctionalTests.Endpoints.CartEndpoints;

public class GetCartItemsTests(CustomWebApplicationFactory factory) : EndpointTestBase(factory)
{
    private async Task<int> CreateOfferInDbAndReturnId(string sellerEmail, string title = "Test Offer", decimal price = 10000m)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var seller = await db.Set<User>().FirstAsync(u => u.Email == sellerEmail);
        var category = await db.Set<Category>().Include(c => c.PropertyDefinitions).FirstAsync();
        var mileageDef = category.PropertyDefinitions.First(pd => pd.Name == "Mileage");
        var damagedDef = category.PropertyDefinitions.First(pd => pd.Name == "Damaged");

        List<Property> properties =
        [
            mileageDef.CreatePropertyFromString("120000"),
            damagedDef.CreatePropertyFromString("false")
        ];

        var offer = new Offer(title, "Description", price, [], seller, [], 1, category, properties);
        db.Set<Offer>().Add(offer);
        await db.SaveChangesAsync();
        return offer.Id;
    }

    private async Task AddItemToCart(int userId, int offerId, int quantity)
    {
        using var scope = _factory.Services.CreateScope();
        var cartRepo = scope.ServiceProvider.GetRequiredService<ICartRepository>();
        var cart = await cartRepo.GetCartAsync(userId, CancellationToken.None);
        cart.UpdateItemQuantity(offerId, quantity);
        await cartRepo.SaveCartAsync(cart, CancellationToken.None);
    }

    private async Task<int> GetUserIdByEmail(string email)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = await db.Set<User>().FirstAsync(u => u.Email == email);
        return user.Id;
    }

    [Fact]
    public async Task GetCartItems_Unauthenticated_ReturnsUnauthorized()
    {
        ClearAuthHeader();

        var response = await _client.GetAsync("/api/v1/cart/items");

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCartItems_EmptyCart_ReturnsOkWithEmptyList()
    {
        var token = await RegisterAndLogin("emptycart@example.com", "Password123!");
        SetAuthHeader(token);

        var response = await _client.GetAsync("/api/v1/cart/items");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        json.RootElement.GetArrayLength().ShouldBe(0);
    }

    [Fact]
    public async Task GetCartItems_WithItems_ReturnsOkWithItems()
    {
        await RegisterAndLogin("seller_cart@example.com", "Password123!");
        var buyerToken = await RegisterAndLogin("buyer_cart@example.com", "Password123!");
        var buyerId = await GetUserIdByEmail("buyer_cart@example.com");

        var offerId1 = await CreateOfferInDbAndReturnId("seller_cart@example.com", "BMW E46", 15000m);
        var offerId2 = await CreateOfferInDbAndReturnId("seller_cart@example.com", "Audi A4", 20000m);

        await AddItemToCart(buyerId, offerId1, 1);
        await AddItemToCart(buyerId, offerId2, 2);

        SetAuthHeader(buyerToken);
        var response = await _client.GetAsync("/api/v1/cart/items");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        json.RootElement.GetArrayLength().ShouldBe(2);
    }

    [Fact]
    public async Task GetCartItems_DifferentUsers_Isolated()
    {
        await RegisterAndLogin("seller_iso@example.com", "Password123!");
        var user1Token = await RegisterAndLogin("user1_iso@example.com", "Password123!");
        var user2Token = await RegisterAndLogin("user2_iso@example.com", "Password123!");
        var user1Id = await GetUserIdByEmail("user1_iso@example.com");
        var user2Id = await GetUserIdByEmail("user2_iso@example.com");

        var offerId1 = await CreateOfferInDbAndReturnId("seller_iso@example.com", "Offer 1", 5000m);
        var offerId2 = await CreateOfferInDbAndReturnId("seller_iso@example.com", "Offer 2", 8000m);

        await AddItemToCart(user1Id, offerId1, 1);
        await AddItemToCart(user2Id, offerId2, 3);

        SetAuthHeader(user1Token);
        var response1 = await _client.GetAsync("/api/v1/cart/items");
        var body1 = await response1.Content.ReadAsStringAsync();
        var json1 = JsonDocument.Parse(body1);
        json1.RootElement.GetArrayLength().ShouldBe(1);

        SetAuthHeader(user2Token);
        var response2 = await _client.GetAsync("/api/v1/cart/items");
        var body2 = await response2.Content.ReadAsStringAsync();
        var json2 = JsonDocument.Parse(body2);
        json2.RootElement.GetArrayLength().ShouldBe(1);
    }
}
