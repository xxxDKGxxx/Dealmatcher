namespace Dealmatcher.Backend.FunctionalTests.Endpoints.OfferEndpoints;

public class UpdateOfferTests(CustomWebApplicationFactory factory) : EndpointTestBase(factory)
{
    private async Task<(int OfferId, int PropId)> SeedOffer(string sellerEmail)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var seller = await db.Set<User>().FirstAsync(u => u.Email == sellerEmail);

        var category = new Category("Elektronika", "Opis");
        var propertyDef = new NumericPropertyDefinition("RAM", PropertyType.Number);
        category.AddPropertyDefinition(propertyDef);

        db.Set<Category>().Add(category);
        await db.SaveChangesAsync();

        var propValue = new NumericProperty(propertyDef, 16);

        var offer = new Offer(
            "Original Title",
            "Original Description",
            100m,
            ["image1.jpg"],
            seller,
            ["tag1"],
            1,
            category,
            [propValue]
        );

        db.Set<Offer>().Add(offer);
        await db.SaveChangesAsync();

        return (offer.Id, propertyDef.Id);
    }

    [Fact]
    public async Task UpdateOffer_ValidDataOwnedByUser_ReturnsOkAndUpdatesOffer()
    {
        var token = await RegisterAndLogin("owner_update@example.com", "Password123!");
        SetAuthHeader(token);

        var (offerId, propId) = await SeedOffer("owner_update@example.com");

        var updateRequest = new
        {
            Title = "Updated Title",
            Price = 150.99m,
            Properties = new Dictionary<string, string>
            {
                { propId.ToString(), "32" }
            }
        };

        var response = await _client.PatchAsJsonAsync($"/api/v1/offers/{offerId}", (object)updateRequest);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<OfferDto>();
        json?.Title.ShouldBe("Updated Title");
        json?.Properties[propId.ToString()].ShouldBe("32");
    }

    [Fact]
    public async Task UpdateOffer_Twice_DoesNotCreateDuplicatePropertiesInDatabase()
    {
        // Arrange
        var token = await RegisterAndLogin("repro_bug@example.com", "Password123!");
        SetAuthHeader(token);

        var (offerId, propId) = await SeedOffer("repro_bug@example.com");

        // First update
        var update1 = new
        {
            Properties = new Dictionary<string, string> { { propId.ToString(), "32" } }
        };
        var resp1 = await _client.PatchAsJsonAsync($"/api/v1/offers/{offerId}", (object)update1);
        resp1.StatusCode.ShouldBe(HttpStatusCode.OK);

        // Second update
        var update2 = new
        {
            Properties = new Dictionary<string, string> { { propId.ToString(), "64" } }
        };
        var resp2 = await _client.PatchAsJsonAsync($"/api/v1/offers/{offerId}", (object)update2);
        resp2.StatusCode.ShouldBe(HttpStatusCode.OK);

        // Act
        var response = await _client.GetAsync($"/api/v1/offers/{offerId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var json = await response.Content.ReadFromJsonAsync<OfferDto>();
        json.ShouldNotBeNull();
        json.Properties.Count.ShouldBe(1);
        json.Properties[propId.ToString()].ShouldBe("64");
    }

    [Fact]
    public async Task UpdateOffer_OfferOwnedByOtherUser_ReturnsForbidden()
    {
        await RegisterAndLogin("real_owner_update@example.com", "Password123!");
        var (offerId, propId) = await SeedOffer("real_owner_update@example.com");

        var maliciousUserToken = await RegisterAndLogin("malicious_update@example.com", "Password123!");
        SetAuthHeader(maliciousUserToken);

        var updateRequest = new
        {
            Title = "Hacked Title",
            Properties = new Dictionary<string, string> { { propId.ToString(), "16" } }
        };
        var response = await _client.PatchAsJsonAsync($"/api/v1/offers/{offerId}", (object)updateRequest);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateOffer_InvalidData_ReturnsBadRequest()
    {
        var token = await RegisterAndLogin("owner_invalid_update@example.com", "Password123!");
        SetAuthHeader(token);

        var (offerId, propId) = await SeedOffer("owner_invalid_update@example.com");

        var updateRequest = new
        {
            Title = "",
            Price = -50m,
            Properties = new Dictionary<string, string> { { propId.ToString(), "16" } }
        };

        var response = await _client.PatchAsJsonAsync($"/api/v1/offers/{offerId}", (object)updateRequest);

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
