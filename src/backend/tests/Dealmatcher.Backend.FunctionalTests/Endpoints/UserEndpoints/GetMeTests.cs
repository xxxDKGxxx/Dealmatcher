namespace Dealmatcher.Backend.FunctionalTests.Endpoints.UserEndpoints;

public class GetMeTests(CustomWebApplicationFactory factory) : EndpointTestBase(factory)
{
    [Fact]
    public async Task GetMe_AuthenticatedUser_ReturnsOkWithUserProfile()
    {
        var token = await RegisterAndLogin("me@example.com", "Password123!");
        SetAuthHeader(token);

        var response = await _client.GetAsync("/api/v1/users/me");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var user = await response.Content.ReadFromJsonAsync<JsonDocument>();
        user.ShouldNotBeNull();

        var root = user.RootElement;
        root.GetProperty("email").GetString().ShouldBe("me@example.com");
        root.GetProperty("name").GetString().ShouldBe("Test");
        root.GetProperty("surname").GetString().ShouldBe("User");
        root.GetProperty("id").GetInt32().ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task GetMe_UnauthenticatedUser_ReturnsUnauthorized()
    {
        ClearAuthHeader();

        var response = await _client.GetAsync("/api/v1/users/me");

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMe_BannedUser_ReturnsUnauthorized()
    {
        var token = await RegisterAndLogin("banned_me@example.com", "Password123!");

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var user = db.Set<User>().First(u => u.Email == "banned_me@example.com");
            user.BanUser("", 0, null);
            await db.SaveChangesAsync();
        }

        SetAuthHeader(token);

        var response = await _client.GetAsync("/api/v1/users/me");

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
