namespace Dealmatcher.Backend.FunctionalTests.Endpoints.OfferEndpoints;

public class DeleteOfferTests(CustomWebApplicationFactory factory) : EndpointTestBase(factory)
{
    private async Task<int> SeedOffer(string sellerEmail)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var seller = await db.Set<User>().FirstAsync(u => u.Email == sellerEmail);
        var category = await db.Set<Category>().FirstAsync();

        var offer = new Offer(
            "Test Offer to Delete",
            "Test Description",
            100m,
            ["image1.jpg"],
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
    public async Task DeleteOffer_ExistingOfferOwnedByUser_ReturnsNoContent()
    {
        var token = await RegisterAndLogin("owner_delete@example.com", "Password123!");
        SetAuthHeader(token);

        var offerId = await SeedOffer("owner_delete@example.com");

        var response = await _client.DeleteAsync($"/api/v1/offers/{offerId}");

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteOffer_OfferOwnedByOtherUser_ReturnsForbidden()
    {
        await RegisterAndLogin("real_owner_delete@example.com", "Password123!");
        var offerId = await SeedOffer("real_owner_delete@example.com");

        var maliciousUserToken = await RegisterAndLogin("malicious_delete@example.com", "Password123!");
        SetAuthHeader(maliciousUserToken);

        var response = await _client.DeleteAsync($"/api/v1/offers/{offerId}");

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteOffer_NonExistingOffer_ReturnsNotFound()
    {
        var token = await RegisterAndLogin("user_delete_notfound@example.com", "Password123!");
        SetAuthHeader(token);

        var response = await _client.DeleteAsync("/api/v1/offers/99999");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteOffer_UnauthenticatedUser_ReturnsUnauthorized()
    {
        ClearAuthHeader();

        var response = await _client.DeleteAsync("/api/v1/offers/1");

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
