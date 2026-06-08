namespace Dealmatcher.Backend.FunctionalTests.Endpoints.Bans;

public class CreateBanTests(CustomWebApplicationFactory factory) : EndpointTestBase(factory)
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

    [Fact]
    public async Task CreateBan_ValidDataAsAdmin_ReturnsCreated()
    {
        // Arrange
        var adminToken = await RegisterAndLoginAsAdmin("admin_ban1@example.com", "Password123!");
        await RegisterAndLogin("user_ban1@example.com", "Password123!");
        var targetUserId = await GetUserIdByEmail("user_ban1@example.com");

        SetAuthHeader(adminToken);

        var request = new CreateBanRequest(targetUserId, "Oszustwo w ofercie", DateTime.UtcNow.AddDays(30));

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/bans", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        var responseBan = await response.Content.ReadFromJsonAsync<BanDto>();
        responseBan.ShouldNotBeNull();
        responseBan.Reason.ShouldBe("Oszustwo w ofercie");
        responseBan.IsActive.ShouldBeTrue();
        responseBan.UserId.ShouldBe(targetUserId);
    }

    [Fact]
    public async Task CreateBan_AsRegularUser_ReturnsForbidden()
    {
        // Arrange
        var regularUserToken = await RegisterAndLogin("regular_user@example.com", "Password123!");
        await RegisterAndLogin("target_user@example.com", "Password123!");
        var targetUserId = await GetUserIdByEmail("target_user@example.com");

        SetAuthHeader(regularUserToken);

        var request = new CreateBanRequest(targetUserId, "Spam", null);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/bans", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateBan_Unauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthHeader();
        var request = new CreateBanRequest(1, "Spam", null);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/bans", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateBan_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var adminToken = await RegisterAndLoginAsAdmin("admin_ban2@example.com", "Password123!");
        SetAuthHeader(adminToken);

        var request = new CreateBanRequest(99999, "Nieznany user", null);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/bans", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateBan_UserAlreadyBanned_ReturnsConflict()
    {
        // Arrange
        var adminToken = await RegisterAndLoginAsAdmin("admin_ban3@example.com", "Password123!");
        await RegisterAndLogin("user_ban3@example.com", "Password123!");
        var targetUserId = await GetUserIdByEmail("user_ban3@example.com");

        SetAuthHeader(adminToken);
        var request = new CreateBanRequest(targetUserId, "Pierwszy ban", null);

        // Nakładamy pierwszego bana
        await _client.PostAsJsonAsync("/api/v1/bans", request);

        // Act
        var secondRequest = new CreateBanRequest(targetUserId, "Drugi ban", null);
        var response = await _client.PostAsJsonAsync("/api/v1/bans", secondRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }
}
