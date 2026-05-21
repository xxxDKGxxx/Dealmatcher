namespace Dealmatcher.Backend.FunctionalTests.Endpoints.OfferEndpoints;

public class ChangeStatusTests(CustomWebApplicationFactory factory) : EndpointTestBase(factory)
{
    private async Task<int> SeedOffer(string sellerEmail, bool makeActive = false)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var seller = await db.Set<User>().FirstAsync(u => u.Email == sellerEmail);

        var category = new Category("Kategoria Status", "Opis");
        db.Set<Category>().Add(category);
        await db.SaveChangesAsync();

        var offer = new Offer(
            "Testowa oferta do zmiany statusu",
            "Opis",
            100m,
            ["image1.jpg"],
            seller,
            ["tag1"],
            1,
            category,
            []
        );

        if (makeActive)
        {
            offer.Activate();
        }

        db.Set<Offer>().Add(offer);
        await db.SaveChangesAsync();

        return offer.Id;
    }

    [Fact]
    public async Task ChangeStatus_AdminSetsActive_ReturnsOk()
    {
        // Arrange
        var ownerEmail = "owner_draft@example.com";
        await RegisterAndLogin(ownerEmail, "Password123!");
        var offerId = await SeedOffer(ownerEmail, makeActive: false);

        var adminEmail = "admin_activator@example.com";
        var adminPassword = "AdminPassword123!";
        await RegisterAndLogin(adminEmail, adminPassword);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var adminUser = await db.Set<User>().FirstAsync(u => u.Email == adminEmail);
            adminUser.GrantAdminPrivileges();
            await db.SaveChangesAsync();
        }

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/users/login", new { Email = adminEmail, Password = adminPassword });
        var json = await loginResponse.Content.ReadFromJsonAsync<JsonDocument>();
        var adminToken = json!.RootElement.GetProperty("accessToken").GetString()!;
        SetAuthHeader(adminToken);

        var updateRequest = new { Status = "ACTIVE" };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/offers/{offerId}/status", updateRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<OfferDto>();
        result?.Status.ShouldBe("ACTIVE");
    }

    [Fact]
    public async Task ChangeStatus_OwnerSetsSold_ReturnsOk()
    {
        // Arrange
        var ownerEmail = "owner_sold@example.com";
        var token = await RegisterAndLogin(ownerEmail, "Password123!");
        SetAuthHeader(token);

        var offerId = await SeedOffer(ownerEmail, makeActive: true);

        var updateRequest = new { Status = "SOLD" };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/offers/{offerId}/status", updateRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<OfferDto>();
        result?.Status.ShouldBe("SOLD");
    }

    [Fact]
    public async Task ChangeStatus_NormalUserSetsActive_ReturnsForbidden()
    {
        // Arrange
        var email = "normal_active@example.com";
        var token = await RegisterAndLogin(email, "Password123!");
        SetAuthHeader(token);

        var offerId = await SeedOffer(email, makeActive: false);

        var updateRequest = new { Status = "ACTIVE" };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/offers/{offerId}/status", updateRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ChangeStatus_NonOwnerSetsSold_ReturnsForbidden()
    {
        // Arrange
        var ownerEmail = "owner_real@example.com";
        await RegisterAndLogin(ownerEmail, "Password123!");
        var offerId = await SeedOffer(ownerEmail, makeActive: true);

        var maliciousToken = await RegisterAndLogin("malicious_sold@example.com", "Password123!");
        SetAuthHeader(maliciousToken);

        var updateRequest = new { Status = "SOLD" };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/offers/{offerId}/status", updateRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ChangeStatus_InvalidStatus_ReturnsBadRequest()
    {
        // Arrange
        var email = "invalid_status@example.com";
        var token = await RegisterAndLogin(email, "Password123!");
        SetAuthHeader(token);
        var offerId = await SeedOffer(email);

        var updateRequest = new { Status = "INVALID_STATUS" };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/offers/{offerId}/status", updateRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ChangeStatus_PromotedStatus_ReturnsBadRequest()
    {
        // Arrange
        var email = "promoted_status@example.com";
        var token = await RegisterAndLogin(email, "Password123!");
        SetAuthHeader(token);
        var offerId = await SeedOffer(email);

        var updateRequest = new { Status = "PROMOTED" };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/offers/{offerId}/status", updateRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ChangeStatus_NonExistingOffer_ReturnsNotFound()
    {
        // Arrange
        var token = await RegisterAndLogin("notfound_status@example.com", "Password123!");
        SetAuthHeader(token);

        var updateRequest = new { Status = "SOLD" };

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/offers/99999/status", updateRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ChangeStatus_Unauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthHeader();
        var updateRequest = new { Status = "SOLD" };

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/offers/1/status", updateRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
