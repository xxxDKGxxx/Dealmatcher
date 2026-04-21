namespace Dealmatcher.Backend.FunctionalTests.Endpoints.OfferEndpoints;

public class UpdateOfferTests(CustomWebApplicationFactory factory) : EndpointTestBase(factory)
{
    private async Task<int> SeedOffer(string sellerEmail)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var seller = await db.Set<User>().FirstAsync(u => u.Email == sellerEmail);
        var category = await db.Set<Category>().FirstAsync();

        var offer = new Offer(
            "Original Title",
            "Original Description",
            100m,
            ["image1.jpg", "image2.jpg"],
            seller,
            ["tag1"],
            1,
            category,
            []
        );

        db.Set<Offer>().Add(offer);
        await db.SaveChangesAsync();

        return offer.Id;
    }

    [Fact]
    public async Task UpdateOffer_ValidDataOwnedByUser_ReturnsOkAndUpdatesOffer()
    {
        var token = await RegisterAndLogin("owner_update@example.com", "Password123!");
        SetAuthHeader(token);

        var offerId = await SeedOffer("owner_update@example.com");

        var updateRequest = new
        {
            Title = "Updated Title",
            Price = 150.99m,
            Images = new[] { "image1.jpg" }
        };

        var response = await _client.PatchAsJsonAsync($"/api/v1/offers/{offerId}", updateRequest);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonDocument>();
        json.ShouldNotBeNull();

        var root = json.RootElement;
        root.GetProperty("id").GetInt32().ShouldBe(offerId);
        root.GetProperty("title").GetString().ShouldBe("Updated Title");
        root.GetProperty("price").GetDecimal().ShouldBe(150.99m);
        root.GetProperty("description").GetString().ShouldBe("Original Description");
        root.GetProperty("images").GetArrayLength().ShouldBe(1);
        root.GetProperty("status").GetString().ShouldBe("DRAFT");
    }

    [Fact]
    public async Task UpdateOffer_OfferOwnedByOtherUser_ReturnsForbidden()
    {
        await RegisterAndLogin("real_owner_update@example.com", "Password123!");
        var offerId = await SeedOffer("real_owner_update@example.com");

        var maliciousUserToken = await RegisterAndLogin("malicious_update@example.com", "Password123!");
        SetAuthHeader(maliciousUserToken);

        var updateRequest = new { Title = "Hacked Title" };
        var response = await _client.PatchAsJsonAsync($"/api/v1/offers/{offerId}", updateRequest);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateOffer_InvalidData_ReturnsBadRequest()
    {
        var token = await RegisterAndLogin("owner_invalid_update@example.com", "Password123!");
        SetAuthHeader(token);

        var offerId = await SeedOffer("owner_invalid_update@example.com");

        var updateRequest = new
        {
            Title = "",
            Price = -50m
        };

        var response = await _client.PatchAsJsonAsync($"/api/v1/offers/{offerId}", updateRequest);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateOffer_NonExistingOffer_ReturnsNotFound()
    {
        var token = await RegisterAndLogin("user_update_notfound@example.com", "Password123!");
        SetAuthHeader(token);

        var updateRequest = new { Title = "New Title" };
        var response = await _client.PatchAsJsonAsync("/api/v1/offers/99999", updateRequest);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateOffer_UnauthenticatedUser_ReturnsUnauthorized()
    {
        ClearAuthHeader();

        var updateRequest = new { Title = "New Title" };
        var response = await _client.PatchAsJsonAsync("/api/v1/offers/1", updateRequest);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
