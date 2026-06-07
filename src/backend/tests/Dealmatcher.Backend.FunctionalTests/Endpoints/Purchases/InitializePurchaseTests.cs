namespace Dealmatcher.Backend.FunctionalTests.Endpoints.Purchases;

public class InitializePurchaseTests(CustomWebApplicationFactory factory) : EndpointTestBase(factory)
{
    private const string DeliveryMethodId = "example_courier";
    private const string PaymentMethodId = "ExampleProviderId";

    private async Task<int> SeedActiveOffer(string sellerEmail, int availability = 5, decimal price = 100m)
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

        var offer = new Offer("Oferta do zakupu", "Opis", price, [], seller, [], availability, category, properties);
        offer.Activate();

        db.Set<Offer>().Add(offer);
        await db.SaveChangesAsync();
        return offer.Id;
    }

    private static object BuildRequest(int offerId, int quantity = 1, string? deliveryMethodId = null, string? paymentMethodId = null) =>
        new
        {
            offerId,
            deliveryMethodId = deliveryMethodId ?? DeliveryMethodId,
            paymentMethodId = paymentMethodId ?? PaymentMethodId,
            quantity
        };

    [Fact]
    public async Task Initialize_ValidRequest_ReturnsOkWithRedirectUrl()
    {
        await RegisterAndLogin("seller_init@example.com", "Password123!");
        var offerId = await SeedActiveOffer("seller_init@example.com");

        var buyerToken = await RegisterAndLogin("buyer_init@example.com", "Password123!");
        SetAuthHeader(buyerToken);

        var response = await _client.PostAsJsonAsync("/api/v1/purchases/initialize", BuildRequest(offerId));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        json.RootElement.TryGetProperty("redirectUrl", out var redirectUrl).ShouldBeTrue();
        redirectUrl.GetString().ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task Initialize_DecrementsAvailability()
    {
        await RegisterAndLogin("seller_avail@example.com", "Password123!");
        var offerId = await SeedActiveOffer("seller_avail@example.com", availability: 5);

        var buyerToken = await RegisterAndLogin("buyer_avail@example.com", "Password123!");
        SetAuthHeader(buyerToken);

        var response = await _client.PostAsJsonAsync("/api/v1/purchases/initialize", BuildRequest(offerId, quantity: 2));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var offer = await db.Set<Offer>().FirstAsync(o => o.Id == offerId);
        offer.Availability.ShouldBe(3);
    }

    [Fact]
    public async Task Initialize_CreatesPurchaseInDb()
    {
        await RegisterAndLogin("seller_dbcheck@example.com", "Password123!");
        var offerId = await SeedActiveOffer("seller_dbcheck@example.com");

        var buyerToken = await RegisterAndLogin("buyer_dbcheck@example.com", "Password123!");
        SetAuthHeader(buyerToken);

        await _client.PostAsJsonAsync("/api/v1/purchases/initialize", BuildRequest(offerId));

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var purchase = await db.Set<Purchase>()
            .Include(p => p.Offer)
            .FirstOrDefaultAsync(p => p.Offer.Id == offerId);

        purchase.ShouldNotBeNull();
        purchase.Quantity.ShouldBe(1);
        purchase.PaymentSessionId.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task Initialize_Unauthenticated_ReturnsUnauthorized()
    {
        ClearAuthHeader();

        var response = await _client.PostAsJsonAsync("/api/v1/purchases/initialize", BuildRequest(1));

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Initialize_OfferNotFound_ReturnsNotFound()
    {
        var buyerToken = await RegisterAndLogin("buyer_notfound@example.com", "Password123!");
        SetAuthHeader(buyerToken);

        var response = await _client.PostAsJsonAsync("/api/v1/purchases/initialize", BuildRequest(999999));

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Initialize_OwnOffer_ReturnsConflict()
    {
        var sellerToken = await RegisterAndLogin("seller_own@example.com", "Password123!");
        var offerId = await SeedActiveOffer("seller_own@example.com");
        SetAuthHeader(sellerToken);

        var response = await _client.PostAsJsonAsync("/api/v1/purchases/initialize", BuildRequest(offerId));

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Initialize_InsufficientAvailability_ReturnsConflict()
    {
        await RegisterAndLogin("seller_short@example.com", "Password123!");
        var offerId = await SeedActiveOffer("seller_short@example.com", availability: 1);

        var buyerToken = await RegisterAndLogin("buyer_short@example.com", "Password123!");
        SetAuthHeader(buyerToken);

        var response = await _client.PostAsJsonAsync("/api/v1/purchases/initialize", BuildRequest(offerId, quantity: 5));

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Initialize_InvalidPaymentMethod_ReturnsBadRequest()
    {
        await RegisterAndLogin("seller_badpay@example.com", "Password123!");
        var offerId = await SeedActiveOffer("seller_badpay@example.com");

        var buyerToken = await RegisterAndLogin("buyer_badpay@example.com", "Password123!");
        SetAuthHeader(buyerToken);

        var response = await _client.PostAsJsonAsync(
            "/api/v1/purchases/initialize",
            BuildRequest(offerId, paymentMethodId: "non_existing_provider"));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Initialize_InvalidDeliveryMethod_ReturnsBadRequest()
    {
        await RegisterAndLogin("seller_baddel@example.com", "Password123!");
        var offerId = await SeedActiveOffer("seller_baddel@example.com");

        var buyerToken = await RegisterAndLogin("buyer_baddel@example.com", "Password123!");
        SetAuthHeader(buyerToken);

        var response = await _client.PostAsJsonAsync(
            "/api/v1/purchases/initialize",
            BuildRequest(offerId, deliveryMethodId: "non_existing_courier"));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Initialize_QuantityZero_ReturnsBadRequest()
    {
        await RegisterAndLogin("seller_qty@example.com", "Password123!");
        var offerId = await SeedActiveOffer("seller_qty@example.com");

        var buyerToken = await RegisterAndLogin("buyer_qty@example.com", "Password123!");
        SetAuthHeader(buyerToken);

        var response = await _client.PostAsJsonAsync("/api/v1/purchases/initialize", BuildRequest(offerId, quantity: 0));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Initialize_DraftOffer_ReturnsConflict()
    {
        await RegisterAndLogin("seller_draft@example.com", "Password123!");

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var seller = await db.Set<User>().FirstAsync(u => u.Email == "seller_draft@example.com");
        var category = await db.Set<Category>().Include(c => c.PropertyDefinitions).FirstAsync();
        var offer = new Offer("Draft Offer", "Desc", 100m, [], seller, [], 5, category, []);
        db.Set<Offer>().Add(offer);
        await db.SaveChangesAsync();

        var buyerToken = await RegisterAndLogin("buyer_draft@example.com", "Password123!");
        SetAuthHeader(buyerToken);

        var response = await _client.PostAsJsonAsync("/api/v1/purchases/initialize", BuildRequest(offer.Id));

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Initialize_NegativeQuantity_ReturnsBadRequest()
    {
        await RegisterAndLogin("seller_neg@example.com", "Password123!");
        var offerId = await SeedActiveOffer("seller_neg@example.com");

        var buyerToken = await RegisterAndLogin("buyer_neg@example.com", "Password123!");
        SetAuthHeader(buyerToken);

        var response = await _client.PostAsJsonAsync("/api/v1/purchases/initialize", BuildRequest(offerId, quantity: -1));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
