namespace Dealmatcher.Backend.FunctionalTests.Endpoints.AdminEndpoints.Activities;

public class GetUserActivityTests(CustomWebApplicationFactory factory) : EndpointTestBase(factory)
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

    private async Task<int> GetUserIdByEmail(string email)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = await db.Set<User>().FirstAsync(u => u.Email == email);
        return user.Id;
    }

    private async Task SeedActivity(int userId, ActivityAction action, DateTime createdAt, int? offerId = null)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var user = await db.Set<User>().FirstAsync(u => u.Id == userId);
        Offer? offer = null;
        if (offerId is not null)
            offer = await db.Set<Offer>().FirstOrDefaultAsync(o => o.Id == offerId);

        var activity = new Activity(
            user,
            offer,
            action,
            new Dictionary<string, string> { ["test"] = "value" },
            System.Net.IPAddress.Parse("127.0.0.1"));

        db.Set<Activity>().Add(activity);
        await db.SaveChangesAsync();

        db.Entry(activity).Property("CreatedAt").CurrentValue = createdAt;
        await db.SaveChangesAsync();
    }

    [Fact]
    public async Task GetUserActivity_ValidAdmin_ReturnsOkWithActivities()
    {
        var adminToken = await RegisterAndLoginAsAdmin("admin_act1@example.com", "Password123!");
        await RegisterAndLogin("user_act1@example.com", "Password123!");
        var userId = await GetUserIdByEmail("user_act1@example.com");

        await SeedActivity(userId, ActivityAction.Login, DateTime.UtcNow.AddDays(-1));
        await SeedActivity(userId, ActivityAction.Create, DateTime.UtcNow.AddDays(-1));

        SetAuthHeader(adminToken);
        var from = DateTime.UtcNow.AddDays(-7).ToString("O");
        var to = DateTime.UtcNow.ToString("O");
        var response = await _client.GetAsync($"/api/v1/admin/activity/user/{userId}?From={from}&To={to}");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        json.RootElement.GetArrayLength().ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task GetUserActivity_FilterByDateRange_ReturnsOnlyInRange()
    {
        var adminToken = await RegisterAndLoginAsAdmin("admin_act2@example.com", "Password123!");
        await RegisterAndLogin("user_act2@example.com", "Password123!");
        var userId = await GetUserIdByEmail("user_act2@example.com");

        await SeedActivity(userId, ActivityAction.Login, DateTime.UtcNow.AddDays(-10));
        await SeedActivity(userId, ActivityAction.Create, DateTime.UtcNow.AddDays(-2));
        await SeedActivity(userId, ActivityAction.Delete, DateTime.UtcNow.AddDays(-1));

        SetAuthHeader(adminToken);
        var from = DateTime.UtcNow.AddDays(-3).ToString("O");
        var to = DateTime.UtcNow.ToString("O");
        var response = await _client.GetAsync($"/api/v1/admin/activity/user/{userId}?From={from}&To={to}");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        json.RootElement.GetArrayLength().ShouldBe(2);
    }

    [Fact]
    public async Task GetUserActivity_NoActivities_ReturnsOkWithEmptyList()
    {
        var adminToken = await RegisterAndLoginAsAdmin("admin_act3@example.com", "Password123!");
        await RegisterAndLogin("user_act3@example.com", "Password123!");
        var userId = await GetUserIdByEmail("user_act3@example.com");

        SetAuthHeader(adminToken);
        var from = DateTime.UtcNow.AddDays(-7).ToString("O");
        var to = DateTime.UtcNow.ToString("O");
        var response = await _client.GetAsync($"/api/v1/admin/activity/user/{userId}?From={from}&To={to}");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        json.RootElement.GetArrayLength().ShouldBe(0);
    }

    [Fact]
    public async Task GetUserActivity_Unauthenticated_ReturnsUnauthorized()
    {
        ClearAuthHeader();

        var response = await _client.GetAsync("/api/v1/admin/activity/user/1");

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUserActivity_RegularUser_ReturnsForbidden()
    {
        var userToken = await RegisterAndLogin("regular_act@example.com", "Password123!");
        SetAuthHeader(userToken);

        var response = await _client.GetAsync("/api/v1/admin/activity/user/1");

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetUserActivity_UserNotFound_ReturnsNotFound()
    {
        var adminToken = await RegisterAndLoginAsAdmin("admin_act4@example.com", "Password123!");
        SetAuthHeader(adminToken);

        var response = await _client.GetAsync("/api/v1/admin/activity/user/99999?From=2026-01-01T00:00:00Z&To=2026-12-31T23:59:59Z");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetUserActivity_CorrectStructure()
    {
        var adminToken = await RegisterAndLoginAsAdmin("admin_act5@example.com", "Password123!");
        await RegisterAndLogin("user_act5@example.com", "Password123!");
        var userId = await GetUserIdByEmail("user_act5@example.com");

        await SeedActivity(userId, ActivityAction.Login, DateTime.UtcNow.AddHours(-1));

        SetAuthHeader(adminToken);
        var from = DateTime.UtcNow.AddDays(-1).ToString("O");
        var to = DateTime.UtcNow.ToString("O");
        var response = await _client.GetAsync($"/api/v1/admin/activity/user/{userId}?From={from}&To={to}");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        var first = json.RootElement[0];

        first.TryGetProperty("id", out _).ShouldBeTrue();
        first.TryGetProperty("userId", out _).ShouldBeTrue();
        first.TryGetProperty("action", out _).ShouldBeTrue();
        first.TryGetProperty("details", out _).ShouldBeTrue();
        first.TryGetProperty("ipAddress", out _).ShouldBeTrue();
        first.TryGetProperty("createdAt", out _).ShouldBeTrue();
    }

    [Fact]
    public async Task GetUserActivity_DefaultDateRange_ReturnsOk()
    {
        var adminToken = await RegisterAndLoginAsAdmin("admin_act6@example.com", "Password123!");
        await RegisterAndLogin("user_act6@example.com", "Password123!");
        var userId = await GetUserIdByEmail("user_act6@example.com");

        SetAuthHeader(adminToken);
        var response = await _client.GetAsync($"/api/v1/admin/activity/user/{userId}");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
