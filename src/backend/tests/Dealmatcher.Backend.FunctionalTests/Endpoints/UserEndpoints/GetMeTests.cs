namespace Dealmatcher.Backend.FunctionalTests.Endpoints.UserEndpoints;

public class GetMeTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly CustomWebApplicationFactory _factory = factory;

    private async Task SeedUser(string email, string password)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        var user = new User(email, hasher.HashPassword(password), "Test", "User");
        db.Set<User>().Add(user);
        await db.SaveChangesAsync();
    }

    private async Task<string> GetAccessToken(string email, string password)
    {
        var response = await _client.PostAsJsonAsync("/api/v1/users/login", new
        {
            Email = email,
            Password = password
        });

        var json = await response.Content.ReadFromJsonAsync<JsonDocument>();
        return json!.RootElement.GetProperty("accessToken").GetString()!;
    }

    [Fact]
    public async Task GetMe_AuthenticatedUser_ReturnsOkWithUserProfile()
    {
        // Arrange
        const string email = "me@example.com";
        const string password = "Password123!";
        await SeedUser(email, password);
        var token = await GetAccessToken(email, password);

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/users/me");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var user = await response.Content.ReadFromJsonAsync<JsonDocument>();
        user.ShouldNotBeNull();

        var root = user.RootElement;
        root.GetProperty("email").GetString().ShouldBe(email);
        root.GetProperty("name").GetString().ShouldBe("Test");
        root.GetProperty("surname").GetString().ShouldBe("User");
        root.GetProperty("id").GetInt32().ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task GetMe_UnauthenticatedUser_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/users/me");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMe_BannedUser_ReturnsUnauthorized()
    {
        // Arrange
        const string email = "banned_me@example.com";
        const string password = "Password123!";
        await SeedUser(email, password);
        var token = await GetAccessToken(email, password);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var user = db.Set<User>().First(u => u.Email == email);
            user.BanUser();
            await db.SaveChangesAsync();
        }

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/users/me");

        // Assert
        // Usually, a banned user would return Unauthorized or Forbidden
        // FastEndpoints' authorization handles this. Let's see how GetMe handles it.
        // GetMe says:
        /*
        if (result.Status == ResultStatus.NotFound)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }
        */
        // If the query handler filters out banned users, it will return NotFound.
        // Let's check GetUserProfileQuery handler.
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
