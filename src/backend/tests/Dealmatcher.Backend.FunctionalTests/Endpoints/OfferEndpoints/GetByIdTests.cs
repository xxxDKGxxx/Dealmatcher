namespace Dealmatcher.Backend.FunctionalTests.Endpoints.OfferEndpoints;

public class GetByIdTests(CustomWebApplicationFactory factory) : EndpointTestBase(factory)
{
    [Fact]
    public async Task GetOffer_ExistingOffer_ReturnsOkWithOfferDetails()
    {
        // Arrange
        var offerId = await SeedOffer();

        // Act
        var response = await _client.GetAsync($"/api/v1/offers/{offerId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var offer = await response.Content.ReadFromJsonAsync<OfferDto>();
        offer.ShouldNotBeNull();
        offer.Id.ShouldBe(offerId);
        offer.Title.ShouldBe("Test Offer");
        offer.Price.ShouldBe(100);
    }

    [Fact]
    public async Task GetOffer_NonExistingOffer_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/offers/999");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    private async Task<int> SeedOffer()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var user = new User(
            "seller@example.com",
            passwordHasher.HashPassword("Password123!"),
            "Seller",
            "User"
        );
        db.Set<User>().Add(user);

        var category = db.Set<Category>().First(c => c.Name == "Cars");

        var offer = new OfferEntity(
            "Test Offer",
            "Test Description",
            100,
            ["image1.jpg"],
            user,
            ["tag1"],
            1,
            category,
            []
        );

        db.Set<OfferEntity>().Add(offer);

        await db.SaveChangesAsync();

        return offer.Id;
    }
}
