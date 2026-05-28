namespace Dealmatcher.Backend.FunctionalTests.Endpoints.AdminEndpoints;

public class AdminGetUsersTests(CustomWebApplicationFactory factory) : EndpointTestBase(factory)
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

    [Fact]
    public async Task AdminGetUsers_ValidAdmin_ReturnsOkWithUsers()
    {
        var adminToken = await RegisterAndLoginAsAdmin("admin_users@example.com", "Password123!");
        await RegisterAndLogin("user1_list@example.com", "Password123!");
        await RegisterAndLogin("user2_list@example.com", "Password123!");

        SetAuthHeader(adminToken);
        var response = await _client.GetAsync("/api/v1/admin/users?Page=1&Limit=20&Status=ACTIVE");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        json.RootElement.GetProperty("items").GetArrayLength().ShouldBeGreaterThanOrEqualTo(2);
        json.RootElement.GetProperty("total").GetInt32().ShouldBeGreaterThanOrEqualTo(2);
        json.RootElement.GetProperty("page").GetInt32().ShouldBe(1);
    }

    [Fact]
    public async Task AdminGetUsers_Pagination_ReturnsCorrectPage()
    {
        var adminToken = await RegisterAndLoginAsAdmin("admin_userspage@example.com", "Password123!");
        for (int i = 0; i < 5; i++)
            await RegisterAndLogin($"paginated_user{i}@example.com", "Password123!");

        SetAuthHeader(adminToken);
        var response = await _client.GetAsync("/api/v1/admin/users?Page=1&Limit=2&Status=ACTIVE");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        json.RootElement.GetProperty("items").GetArrayLength().ShouldBe(2);
        json.RootElement.GetProperty("pages").GetInt32().ShouldBeGreaterThanOrEqualTo(3);
    }

    [Fact]
    public async Task AdminGetUsers_FilterByStatus_ReturnsOnlyMatching()
    {
        var adminToken = await RegisterAndLoginAsAdmin("admin_filter@example.com", "Password123!");

        SetAuthHeader(adminToken);
        var response = await _client.GetAsync("/api/v1/admin/users?Page=1&Limit=20&Status=ADMIN");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        json.RootElement.GetProperty("items").GetArrayLength().ShouldBeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task AdminGetUsers_Unauthenticated_ReturnsUnauthorized()
    {
        ClearAuthHeader();

        var response = await _client.GetAsync("/api/v1/admin/users?Page=1&Limit=20&Status=ACTIVE");

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AdminGetUsers_RegularUser_ReturnsForbidden()
    {
        var userToken = await RegisterAndLogin("regular_userlist@example.com", "Password123!");
        SetAuthHeader(userToken);

        var response = await _client.GetAsync("/api/v1/admin/users?Page=1&Limit=20&Status=ACTIVE");

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AdminGetUsers_InvalidStatus_ReturnsBadRequest()
    {
        var adminToken = await RegisterAndLoginAsAdmin("admin_invalidstatus@example.com", "Password123!");

        SetAuthHeader(adminToken);
        var response = await _client.GetAsync("/api/v1/admin/users?Page=1&Limit=20&Status=NONEXISTENT");

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AdminGetUsers_InvalidLimit_ReturnsBadRequest()
    {
        var adminToken = await RegisterAndLoginAsAdmin("admin_invalidlimit@example.com", "Password123!");

        SetAuthHeader(adminToken);
        var response = await _client.GetAsync("/api/v1/admin/users?Page=1&Limit=0&Status=ACTIVE");

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AdminGetUsers_InvalidPage_ReturnsBadRequest()
    {
        var adminToken = await RegisterAndLoginAsAdmin("admin_invalidpage@example.com", "Password123!");

        SetAuthHeader(adminToken);
        var response = await _client.GetAsync("/api/v1/admin/users?Page=0&Limit=20&Status=ACTIVE");

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AdminGetUsers_BannedStatus_ReturnsOk()
    {
        var adminToken = await RegisterAndLoginAsAdmin("admin_banned@example.com", "Password123!");

        SetAuthHeader(adminToken);
        var response = await _client.GetAsync("/api/v1/admin/users?Page=1&Limit=20&Status=BANNED");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        json.RootElement.GetProperty("items").GetArrayLength().ShouldBe(0);
    }
}
