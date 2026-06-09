namespace Dealmatcher.Backend.FunctionalTests.Endpoints.Purchases;

public class ProcessPaymentTests(CustomWebApplicationFactory factory) : EndpointTestBase(factory)
{
    private async Task<int> CreateOfferInDb(string sellerEmail, int availability = 5, decimal price = 100m)
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

        var offer = new Offer("Test Offer", "Desc", price, [], seller, [], availability, category, properties);
        offer.Activate();
        db.Set<Offer>().Add(offer);
        await db.SaveChangesAsync();
        return offer.Id;
    }

    private async Task<(int PurchaseId, string SessionId)> CreatePurchaseInDb(string buyerEmail, int offerId, int quantity = 1)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var buyer = await db.Set<User>().FirstAsync(u => u.Email == buyerEmail);
        var offer = await db.Set<Offer>().Include(o => o.Seller).FirstAsync(o => o.Id == offerId);

        offer.ReserveQuantity(quantity);

        var sessionId = $"test_session_{Guid.NewGuid():N}";
        var purchase = new Purchase(buyer, offer, quantity, offer.Price * quantity + 10m, "example_courier", "ExampleProviderId");
        purchase.SetPaymentSession(new PaymentSession("ExampleProvider", sessionId, "https://fake.com", purchase.TotalPrice, "PLN"));

        db.Set<Purchase>().Add(purchase);
        await db.SaveChangesAsync();

        return (purchase.Id, sessionId);
    }

    private async Task<string> GetPurchaseStatus(int purchaseId)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var purchase = await db.Set<Purchase>().FirstAsync(p => p.Id == purchaseId);
        return purchase.Status.Name;
    }

    private async Task<int> GetOfferAvailability(int offerId)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var offer = await db.Set<Offer>().FirstAsync(o => o.Id == offerId);
        return offer.Availability;
    }

    private static StringContent WebhookBody(string providerStatus)
    {
        return new StringContent(
            $$"""{"providerStatus":"{{providerStatus}}"}""",
            System.Text.Encoding.UTF8,
            "application/json");
    }

    [Fact]
    public async Task Webhook_Completed_CompletesPurchase()
    {
        await RegisterAndLogin("seller_wh1@example.com", "Password123!");
        await RegisterAndLogin("buyer_wh1@example.com", "Password123!");
        var offerId = await CreateOfferInDb("seller_wh1@example.com", availability: 5);
        var (purchaseId, sessionId) = await CreatePurchaseInDb("buyer_wh1@example.com", offerId);

        var response = await _client.PostAsync(
            $"/api/v1/purchases/webhook/{sessionId}",
            WebhookBody("COMPLETED"));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var status = await GetPurchaseStatus(purchaseId);
        status.ShouldBe("COMPLETED");
    }

    [Fact]
    public async Task Webhook_Failed_FailsPurchaseAndRestoresAvailability()
    {
        await RegisterAndLogin("seller_wh2@example.com", "Password123!");
        await RegisterAndLogin("buyer_wh2@example.com", "Password123!");
        var offerId = await CreateOfferInDb("seller_wh2@example.com", availability: 5);
        var (purchaseId, sessionId) = await CreatePurchaseInDb("buyer_wh2@example.com", offerId, quantity: 2);

        var availabilityBefore = await GetOfferAvailability(offerId);

        var response = await _client.PostAsync(
            $"/api/v1/purchases/webhook/{sessionId}",
            WebhookBody("FAILED"));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var status = await GetPurchaseStatus(purchaseId);
        status.ShouldBe("FAILED");

        var availabilityAfter = await GetOfferAvailability(offerId);
        availabilityAfter.ShouldBe(availabilityBefore + 2);
    }

    [Fact]
    public async Task Webhook_UnknownSession_ReturnsNotFound()
    {
        var response = await _client.PostAsync(
            "/api/v1/purchases/webhook/nonexistent_session",
            WebhookBody("COMPLETED"));

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Webhook_AlreadyCompleted_ReturnsOkWithoutChanges()
    {
        await RegisterAndLogin("seller_wh3@example.com", "Password123!");
        await RegisterAndLogin("buyer_wh3@example.com", "Password123!");
        var offerId = await CreateOfferInDb("seller_wh3@example.com", availability: 5);
        var (purchaseId, sessionId) = await CreatePurchaseInDb("buyer_wh3@example.com", offerId);

        await _client.PostAsync($"/api/v1/purchases/webhook/{sessionId}", WebhookBody("COMPLETED"));

        var response = await _client.PostAsync(
            $"/api/v1/purchases/webhook/{sessionId}",
            WebhookBody("COMPLETED"));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var status = await GetPurchaseStatus(purchaseId);
        status.ShouldBe("COMPLETED");
    }

    [Fact]
    public async Task Webhook_AlreadyFailed_ReturnsOkWithoutChanges()
    {
        await RegisterAndLogin("seller_wh4@example.com", "Password123!");
        await RegisterAndLogin("buyer_wh4@example.com", "Password123!");
        var offerId = await CreateOfferInDb("seller_wh4@example.com", availability: 5);
        var (purchaseId, sessionId) = await CreatePurchaseInDb("buyer_wh4@example.com", offerId);

        await _client.PostAsync($"/api/v1/purchases/webhook/{sessionId}", WebhookBody("FAILED"));

        var response = await _client.PostAsync(
            $"/api/v1/purchases/webhook/{sessionId}",
            WebhookBody("COMPLETED"));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var status = await GetPurchaseStatus(purchaseId);
        status.ShouldBe("FAILED");
    }

    [Fact]
    public async Task Webhook_InvalidBody_ReturnsBadRequest()
    {
        await RegisterAndLogin("seller_wh5@example.com", "Password123!");
        await RegisterAndLogin("buyer_wh5@example.com", "Password123!");
        var offerId = await CreateOfferInDb("seller_wh5@example.com", availability: 5);
        var (_, sessionId) = await CreatePurchaseInDb("buyer_wh5@example.com", offerId);

        var response = await _client.PostAsync(
            $"/api/v1/purchases/webhook/{sessionId}",
            new StringContent("""{"providerStatus":"GARBAGE_STATUS"}""", System.Text.Encoding.UTF8, "application/json"));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Webhook_PendingStatus_ReturnsOkWithoutChanges()
    {
        await RegisterAndLogin("seller_wh6@example.com", "Password123!");
        await RegisterAndLogin("buyer_wh6@example.com", "Password123!");
        var offerId = await CreateOfferInDb("seller_wh6@example.com", availability: 5);
        var (purchaseId, sessionId) = await CreatePurchaseInDb("buyer_wh6@example.com", offerId);

        var response = await _client.PostAsync(
            $"/api/v1/purchases/webhook/{sessionId}",
            WebhookBody("PENDING"));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var status = await GetPurchaseStatus(purchaseId);
        status.ShouldBe("PENDING");
    }

    [Fact]
    public async Task Webhook_IsAnonymous_DoesNotRequireAuth()
    {
        ClearAuthHeader();

        await RegisterAndLogin("seller_wh7@example.com", "Password123!");
        await RegisterAndLogin("buyer_wh7@example.com", "Password123!");
        var offerId = await CreateOfferInDb("seller_wh7@example.com", availability: 5);
        var (_, sessionId) = await CreatePurchaseInDb("buyer_wh7@example.com", offerId);

        ClearAuthHeader();
        var response = await _client.PostAsync(
            $"/api/v1/purchases/webhook/{sessionId}",
            WebhookBody("COMPLETED"));

        response.StatusCode.ShouldNotBe(HttpStatusCode.Unauthorized);
    }
}
