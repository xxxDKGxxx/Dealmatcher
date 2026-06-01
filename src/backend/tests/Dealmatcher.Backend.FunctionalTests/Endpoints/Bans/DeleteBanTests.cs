namespace Dealmatcher.Backend.FunctionalTests.Endpoints.Bans;

public class DeleteBanTests(CustomWebApplicationFactory factory) : EndpointTestBase(factory)
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

    private async Task<int> CreateUserWithBan(string email, string adminEmail)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var admin = await db.Set<User>().FirstAsync(u => u.Email == adminEmail);
        var targetUser = await db.Set<User>().FirstAsync(u => u.Email == email);

        targetUser.BanUser("Spam", admin, null);
        await db.SaveChangesAsync();

        return targetUser.Bans.First().Id;
    }

    [Fact]
    public async Task DeleteBan_ValidAdmin_ReturnsNoContent()
    {
        // Arrange
        var adminToken = await RegisterAndLoginAsAdmin("admin_del1@example.com", "Password123!");
        await RegisterAndLogin("user_del1@example.com", "Password123!");

        var banId = await CreateUserWithBan("user_del1@example.com", "admin_del1@example.com");

        SetAuthHeader(adminToken);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/bans/{banId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = await db.Set<User>().Include(u => u.Bans).FirstAsync(u => u.Email == "user_del1@example.com");

        user.Bans.First().IsActive.ShouldBeFalse();
        user.Status.ShouldBe(UserStatus.Active);
    }

    [Fact]
    public async Task DeleteBan_AsRegularUser_ReturnsForbidden()
    {
        // Arrange
        _ = await RegisterAndLoginAsAdmin("admin_del2@example.com", "Password123!");
        await RegisterAndLogin("user_del2@example.com", "Password123!");
        var banId = await CreateUserWithBan("user_del2@example.com", "admin_del2@example.com");

        var regularToken = await RegisterAndLogin("regular_del@example.com", "Password123!");
        SetAuthHeader(regularToken);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/bans/{banId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteBan_Unauthenticated_ReturnsUnauthorized()
    {
        ClearAuthHeader();
        var response = await _client.DeleteAsync("/api/v1/bans/1");
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteBan_BanNotFound_ReturnsNotFound()
    {
        var adminToken = await RegisterAndLoginAsAdmin("admin_del3@example.com", "Password123!");
        SetAuthHeader(adminToken);

        var response = await _client.DeleteAsync("/api/v1/bans/999999");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
