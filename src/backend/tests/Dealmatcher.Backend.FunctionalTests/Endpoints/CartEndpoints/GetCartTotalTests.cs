namespace Dealmatcher.Backend.FunctionalTests.Endpoints.CartEndpoints;

public class GetCartTotalTests(CustomWebApplicationFactory factory) : EndpointTestBase(factory)
{
    private async Task<int> CreateOfferInDbAndReturnId(string sellerEmail, decimal price, string title = "Test Offer")
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
    public async Task GetCartTotal_Unauthenticated_ReturnsUnauthorized()
    {
        ClearAuthHeader();

        var response = await _client.GetAsync("/api/v1/cart/total");

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCartTotal_EmptyCart_ReturnsZero()
    {
        var token = await RegisterAndLogin("emptytotal@example.com", "Password123!");
        SetAuthHeader(token);

        var response = await _client.GetAsync("/api/v1/cart/total");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        json.RootElement.GetProperty("totalPrice").GetDecimal().ShouldBe(0m);
        json.RootElement.GetProperty("currency").GetString().ShouldBe("PLN");
    }

    [Fact]
    public async Task GetCartTotal_SingleItem_ReturnsCorrectTotal()
    {
        await RegisterAndLogin("seller_total1@example.com", "Password123!");
        var buyerToken = await RegisterAndLogin("buyer_total1@example.com", "Password123!");
        var buyerId = await GetUserIdByEmail("buyer_total1@example.com");

        var offerId = await CreateOfferInDbAndReturnId("seller_total1@example.com", 250m);
        await AddItemToCart(buyerId, offerId, 3);

        SetAuthHeader(buyerToken);
        var response = await _client.GetAsync("/api/v1/cart/total");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        json.RootElement.GetProperty("totalPrice").GetDecimal().ShouldBe(750m);
    }

    [Fact]
    public async Task GetCartTotal_MultipleItems_ReturnsSumTotal()
    {
        await RegisterAndLogin("seller_total2@example.com", "Password123!");
        var buyerToken = await RegisterAndLogin("buyer_total2@example.com", "Password123!");
        var buyerId = await GetUserIdByEmail("buyer_total2@example.com");

        var offerId1 = await CreateOfferInDbAndReturnId("seller_total2@example.com", 100m, "Offer 1");
        var offerId2 = await CreateOfferInDbAndReturnId("seller_total2@example.com", 50m, "Offer 2");

        await AddItemToCart(buyerId, offerId1, 2);
        await AddItemToCart(buyerId, offerId2, 4);

        SetAuthHeader(buyerToken);
        var response = await _client.GetAsync("/api/v1/cart/total");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        json.RootElement.GetProperty("totalPrice").GetDecimal().ShouldBe(400m);
    }

    [Fact]
    public async Task GetCartTotal_DifferentUsers_Isolated()
    {
        await RegisterAndLogin("seller_total3@example.com", "Password123!");
        var user1Token = await RegisterAndLogin("user1_total3@example.com", "Password123!");
        var user2Token = await RegisterAndLogin("user2_total3@example.com", "Password123!");
        var user1Id = await GetUserIdByEmail("user1_total3@example.com");
        var user2Id = await GetUserIdByEmail("user2_total3@example.com");

        var offerId1 = await CreateOfferInDbAndReturnId("seller_total3@example.com", 100m, "Cheap");
        var offerId2 = await CreateOfferInDbAndReturnId("seller_total3@example.com", 500m, "Expensive");

        await AddItemToCart(user1Id, offerId1, 1);
        await AddItemToCart(user2Id, offerId2, 2);

        SetAuthHeader(user1Token);
        var response1 = await _client.GetAsync("/api/v1/cart/total");
        var json1 = JsonDocument.Parse(await response1.Content.ReadAsStringAsync());
        json1.RootElement.GetProperty("totalPrice").GetDecimal().ShouldBe(100m);

        SetAuthHeader(user2Token);
        var response2 = await _client.GetAsync("/api/v1/cart/total");
        var json2 = JsonDocument.Parse(await response2.Content.ReadAsStringAsync());
        json2.RootElement.GetProperty("totalPrice").GetDecimal().ShouldBe(1000m);
    }
}
