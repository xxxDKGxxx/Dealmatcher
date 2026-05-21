namespace Dealmatcher.Backend.FunctionalTests.Endpoints.AdminEndpoints;

public class AdminGetOffersTests(CustomWebApplicationFactory factory) : EndpointTestBase(factory)
{
    private async Task<int> CreateOfferInDb(string sellerEmail, string title = "Test Offer", decimal price = 1000m)
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

    private async Task GrantAdmin(string email)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = await db.Set<User>().FirstAsync(u => u.Email == email);
        user.GrantAdminPrivileges();
        await db.SaveChangesAsync();
    }

    private async Task ActivateOffer(int offerId)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var offer = await db.Set<Offer>().FirstAsync(o => o.Id == offerId);
        offer.Activate();
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

        var json = await loginResponse.Content.ReadFromJsonAsync<JsonDocument>();
        return json!.RootElement.GetProperty("accessToken").GetString()!;
    }

    [Fact]
    public async Task AdminGetOffers_ValidAdmin_ReturnsOkWithOffers()
    {
        var adminToken = await RegisterAndLoginAsAdmin("admin_page@example.com", "Password123!");
        await RegisterAndLogin("seller_offers@example.com", "Password123!");

        await CreateOfferInDb("seller_offers@example.com", "Offer 1");
        await CreateOfferInDb("seller_offers@example.com", "Offer 2");

        SetAuthHeader(adminToken);
        var response = await _client.GetAsync("/api/v1/admin/offers?Page=1&Limit=20&Status=DRAFT");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        json.RootElement.GetProperty("items").GetArrayLength().ShouldBe(2);
        json.RootElement.GetProperty("total").GetInt32().ShouldBe(2);
        json.RootElement.GetProperty("page").GetInt32().ShouldBe(1);
        json.RootElement.GetProperty("pages").GetInt32().ShouldBe(1);
    }

    [Fact]
    public async Task AdminGetOffers_Pagination_ReturnsCorrectPage()
    {
        var adminToken = await RegisterAndLoginAsAdmin("admin_page@example.com", "Password123!");
        await RegisterAndLogin("seller_page@example.com", "Password123!");

        for (int i = 0; i < 5; i++)
            await CreateOfferInDb("seller_page@example.com", $"Offer {i}");

        SetAuthHeader(adminToken);
        var response = await _client.GetAsync("/api/v1/admin/offers?Page=1&Limit=2&Status=DRAFT");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        json.RootElement.GetProperty("items").GetArrayLength().ShouldBe(2);
        json.RootElement.GetProperty("total").GetInt32().ShouldBe(5);
        json.RootElement.GetProperty("pages").GetInt32().ShouldBe(3);
    }

    [Fact]
    public async Task AdminGetOffers_FilterByStatus_ReturnsOnlyMatching()
    {
        var adminToken = await RegisterAndLoginAsAdmin("admin_page@example.com", "Password123!");
        await RegisterAndLogin("seller_status@example.com", "Password123!");

        await CreateOfferInDb("seller_status@example.com", "Draft Offer");
        var offerId2 = await CreateOfferInDb("seller_status@example.com", "Active Offer");
        await ActivateOffer(offerId2);

        SetAuthHeader(adminToken);

        var draftResponse = await _client.GetAsync("/api/v1/admin/offers?Page=1&Limit=20&Status=DRAFT");
        var draftBody = await draftResponse.Content.ReadAsStringAsync();
        var draftJson = JsonDocument.Parse(draftBody);
        draftJson.RootElement.GetProperty("items").GetArrayLength().ShouldBe(1);

        var activeResponse = await _client.GetAsync("/api/v1/admin/offers?Page=1&Limit=20&Status=ACTIVE");
        var activeBody = await activeResponse.Content.ReadAsStringAsync();
        var activeJson = JsonDocument.Parse(activeBody);
        activeJson.RootElement.GetProperty("items").GetArrayLength().ShouldBe(1);
    }

    [Fact]
    public async Task AdminGetOffers_Unauthenticated_ReturnsUnauthorized()
    {
        ClearAuthHeader();

        var response = await _client.GetAsync("/api/v1/admin/offers?Page=1&Limit=20&Status=DRAFT");

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AdminGetOffers_RegularUser_ReturnsForbidden()
    {
        var userToken = await RegisterAndLogin("regular_user@example.com", "Password123!");
        SetAuthHeader(userToken);

        var response = await _client.GetAsync("/api/v1/admin/offers?Page=1&Limit=20&Status=DRAFT");

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AdminGetOffers_InvalidStatus_ReturnsBadRequest()
    {
        var adminToken = await RegisterAndLoginAsAdmin("admin_page@example.com", "Password123!");

        SetAuthHeader(adminToken);
        var response = await _client.GetAsync("/api/v1/admin/offers?Page=1&Limit=20&Status=NONEXISTENT");

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AdminGetOffers_InvalidLimit_ReturnsBadRequest()
    {
        var adminToken = await RegisterAndLoginAsAdmin("admin_page@example.com", "Password123!");

        SetAuthHeader(adminToken);
        var response = await _client.GetAsync("/api/v1/admin/offers?Page=1&Limit=0&Status=DRAFT");

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AdminGetOffers_InvalidPage_ReturnsBadRequest()
    {
        var adminToken = await RegisterAndLoginAsAdmin("admin_page@example.com", "Password123!");

        SetAuthHeader(adminToken);
        var response = await _client.GetAsync("/api/v1/admin/offers?Page=0&Limit=20&Status=DRAFT");

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AdminGetOffers_NoOffers_ReturnsOkWithEmptyList()
    {
        var adminToken = await RegisterAndLoginAsAdmin("admin_page@example.com", "Password123!");

        SetAuthHeader(adminToken);
        var response = await _client.GetAsync("/api/v1/admin/offers?Page=1&Limit=20&Status=ACTIVE");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        json.RootElement.GetProperty("items").GetArrayLength().ShouldBe(0);
        json.RootElement.GetProperty("total").GetInt32().ShouldBe(0);
    }
}
