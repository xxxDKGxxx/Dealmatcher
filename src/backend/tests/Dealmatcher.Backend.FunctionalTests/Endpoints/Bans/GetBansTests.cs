namespace Dealmatcher.Backend.FunctionalTests.Endpoints.Bans;

public class GetBansTests(CustomWebApplicationFactory factory) : EndpointTestBase(factory)
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

    private async Task<User> GetUserByEmail(string email)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await db.Set<User>().FirstAsync(u => u.Email == email);
    }

    private async Task SeedBan(string targetEmail, string adminEmail, string reason, bool isActive)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var admin = await db.Set<User>().FirstAsync(u => u.Email == adminEmail);
        var targetUser = await db.Set<User>().Include(u => u.Bans).FirstAsync(u => u.Email == targetEmail);

        targetUser.BanUser(reason, admin, null);

        if (!isActive)
        {
            targetUser.RevokeBan(targetUser.Bans.Last().Id);
        }

        await db.SaveChangesAsync();
    }

    [Fact]
    public async Task GetBans_ValidAdmin_ReturnsAllBans()
    {
        // Arrange
        var adminToken = await RegisterAndLoginAsAdmin("admin_list1@example.com", "Password123!");
        await RegisterAndLogin("user_list1@example.com", "Password123!");

        await SeedBan("user_list1@example.com", "admin_list1@example.com", "Spam", true);
        await SeedBan("user_list1@example.com", "admin_list1@example.com", "Stary ban", false);

        SetAuthHeader(adminToken);

        // Act
        var response = await _client.GetAsync("/api/v1/bans");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var bans = await response.Content.ReadFromJsonAsync<List<BanDto>>();

        bans.ShouldNotBeNull();
        bans.Count.ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task GetBans_FilterByActive_ReturnsOnlyActiveBans()
    {
        // Arrange
        var adminToken = await RegisterAndLoginAsAdmin("admin_list2@example.com", "Password123!");
        await RegisterAndLogin("user_list2@example.com", "Password123!");

        await SeedBan("user_list2@example.com", "admin_list2@example.com", "Aktywny ban", true);
        await SeedBan("user_list2@example.com", "admin_list2@example.com", "Wygasly ban", false);

        SetAuthHeader(adminToken);

        // Act
        var response = await _client.GetAsync("/api/v1/bans?active=true");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var bans = await response.Content.ReadFromJsonAsync<List<BanDto>>();

        bans.ShouldNotBeNull();
        bans.ShouldAllBe(b => b.IsActive == true);
        bans.ShouldContain(b => b.Reason == "Aktywny ban");
        bans.ShouldNotContain(b => b.Reason == "Wygasly ban");
    }

    [Fact]
    public async Task GetBans_FilterByUserId_ReturnsBansForSpecificUser()
    {
        // Arrange
        var adminToken = await RegisterAndLoginAsAdmin("admin_list3@example.com", "Password123!");
        await RegisterAndLogin("user_list3_a@example.com", "Password123!");
        await RegisterAndLogin("user_list3_b@example.com", "Password123!");

        await SeedBan("user_list3_a@example.com", "admin_list3@example.com", "Ban User A", true);
        await SeedBan("user_list3_b@example.com", "admin_list3@example.com", "Ban User B", true);

        var userA = await GetUserByEmail("user_list3_a@example.com");

        SetAuthHeader(adminToken);

        // Act
        var response = await _client.GetAsync($"/api/v1/bans?userId={userA.Id}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var bans = await response.Content.ReadFromJsonAsync<List<BanDto>>();

        bans.ShouldNotBeNull();
        bans.ShouldAllBe(b => b.UserId == userA.Id);
    }

    [Fact]
    public async Task GetBans_AsRegularUser_ReturnsForbidden()
    {
        // Arrange
        var regularUserToken = await RegisterAndLogin("regular_list@example.com", "Password123!");
        SetAuthHeader(regularUserToken);

        // Act
        var response = await _client.GetAsync("/api/v1/bans");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetBans_Unauthenticated_ReturnsUnauthorized()
    {
        ClearAuthHeader();
        var response = await _client.GetAsync("/api/v1/bans");
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
