namespace Dealmatcher.Backend.FunctionalTests.Endpoints.AdminEndpoints;

public class GetOfferActivityTests(CustomWebApplicationFactory factory) : EndpointTestBase(factory)
{
    private async Task GrantAdmin(string email)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = await db.Set<User>().FirstAsync(u => u.Email == email);
        user.GrantAdminPrivileges();
        await db.SaveChangesAsync();
    }

    private async Task<string> RegisterAndLoginAsAdmin(string email, string password)
    {
        await RegisterAndLogin(email, password);
        await GrantAdmin(email);

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/users/login", new
        {
            Email = email,
            Password = password
        });

        var body = await loginResponse.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        return json.RootElement.GetProperty("accessToken").GetString()!;
    }

    private async Task<int> CreateOfferInDb(string sellerEmail)
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

        var offer = new Offer("Test Offer", "Description", 1000m, [], seller, [], 1, category, properties);
        db.Set<Offer>().Add(offer);
        await db.SaveChangesAsync();
        return offer.Id;
    }

    private async Task SeedOfferActivity(int offerId, int userId, ActivityAction action)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var user = await db.Set<User>().FirstAsync(u => u.Id == userId);
        var offer = await db.Set<Offer>().FirstAsync(o => o.Id == offerId);

        var activity = new Activity(
            user, offer, action,
            new Dictionary<string, string> { ["test"] = "value" },
            System.Net.IPAddress.Parse("127.0.0.1"));

        db.Set<Activity>().Add(activity);
        await db.SaveChangesAsync();
    }

    private async Task<int> GetUserIdByEmail(string email)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = await db.Set<User>().FirstAsync(u => u.Email == email);
        return user.Id;
    }

    [Fact]
    public async Task GetOfferActivity_ValidAdmin_ReturnsOkWithActivities()
    {
        var adminToken = await RegisterAndLoginAsAdmin("admin_ofact1@example.com", "Password123!");
        await RegisterAndLogin("seller_ofact1@example.com", "Password123!");
        var sellerId = await GetUserIdByEmail("seller_ofact1@example.com");
        var offerId = await CreateOfferInDb("seller_ofact1@example.com");

        await SeedOfferActivity(offerId, sellerId, ActivityAction.Create);
        await SeedOfferActivity(offerId, sellerId, ActivityAction.StatusChange);

        SetAuthHeader(adminToken);
        var from = DateTime.UtcNow.AddDays(-7).ToString("O");
        var to = DateTime.UtcNow.AddDays(1).ToString("O");
        var response = await _client.GetAsync($"/api/v1/admin/activity/offer/{offerId}?From={from}&To={to}");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        json.RootElement.GetArrayLength().ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task GetOfferActivity_NoActivities_ReturnsOkWithEmptyList()
    {
        var adminToken = await RegisterAndLoginAsAdmin("admin_ofact2@example.com", "Password123!");
        await RegisterAndLogin("seller_ofact2@example.com", "Password123!");
        var offerId = await CreateOfferInDb("seller_ofact2@example.com");

        SetAuthHeader(adminToken);
        var from = DateTime.UtcNow.AddDays(-7).ToString("O");
        var to = DateTime.UtcNow.AddDays(1).ToString("O");
        var response = await _client.GetAsync($"/api/v1/admin/activity/offer/{offerId}?From={from}&To={to}");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        json.RootElement.GetArrayLength().ShouldBe(0);
    }

    [Fact]
    public async Task GetOfferActivity_Unauthenticated_ReturnsUnauthorized()
    {
        ClearAuthHeader();

        var response = await _client.GetAsync("/api/v1/admin/activity/offer/1");

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetOfferActivity_RegularUser_ReturnsForbidden()
    {
        var userToken = await RegisterAndLogin("regular_ofact@example.com", "Password123!");
        SetAuthHeader(userToken);

        var response = await _client.GetAsync("/api/v1/admin/activity/offer/1");

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetOfferActivity_OfferNotFound_ReturnsNotFound()
    {
        var adminToken = await RegisterAndLoginAsAdmin("admin_ofact3@example.com", "Password123!");
        SetAuthHeader(adminToken);

        var response = await _client.GetAsync("/api/v1/admin/activity/offer/99999?From=2026-01-01T00:00:00Z&To=2026-12-31T23:59:59Z");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetOfferActivity_DefaultDateRange_ReturnsOk()
    {
        var adminToken = await RegisterAndLoginAsAdmin("admin_ofact4@example.com", "Password123!");
        await RegisterAndLogin("seller_ofact4@example.com", "Password123!");
        var offerId = await CreateOfferInDb("seller_ofact4@example.com");

        SetAuthHeader(adminToken);
        var response = await _client.GetAsync($"/api/v1/admin/activity/offer/{offerId}");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetOfferActivity_CorrectStructure()
    {
        var adminToken = await RegisterAndLoginAsAdmin("admin_ofact5@example.com", "Password123!");
        await RegisterAndLogin("seller_ofact5@example.com", "Password123!");
        var sellerId = await GetUserIdByEmail("seller_ofact5@example.com");
        var offerId = await CreateOfferInDb("seller_ofact5@example.com");

        await SeedOfferActivity(offerId, sellerId, ActivityAction.Create);

        SetAuthHeader(adminToken);
        var from = DateTime.UtcNow.AddDays(-7).ToString("O");
        var to = DateTime.UtcNow.AddDays(1).ToString("O");
        var response = await _client.GetAsync($"/api/v1/admin/activity/offer/{offerId}?From={from}&To={to}");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        var first = json.RootElement[0];

        first.TryGetProperty("id", out _).ShouldBeTrue();
        first.TryGetProperty("userId", out _).ShouldBeTrue();
        first.TryGetProperty("offerId", out _).ShouldBeTrue();
        first.TryGetProperty("action", out _).ShouldBeTrue();
        first.TryGetProperty("details", out _).ShouldBeTrue();
        first.TryGetProperty("ipAddress", out _).ShouldBeTrue();
        first.TryGetProperty("createdAt", out _).ShouldBeTrue();
    }
}
