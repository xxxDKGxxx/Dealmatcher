namespace Dealmatcher.Backend.FunctionalTests.Endpoints.UserEndpoints;

public class PutMeTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
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
    public async Task PutMe_AuthenticatedUser_ReturnsOkWithUpdatedProfile()
    {
        // Arrange
        const string email = "putme@example.com";
        const string password = "Password123!";
        await SeedUser(email, password);
        var token = await GetAccessToken(email, password);

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var request = new
        {
            Name = "UpdatedName",
            Surname = "UpdatedSurname"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/users/me", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var user = await response.Content.ReadFromJsonAsync<JsonDocument>();
        user.ShouldNotBeNull();

        var root = user.RootElement;
        root.GetProperty("email").GetString().ShouldBe(email);
        root.GetProperty("name").GetString().ShouldBe("UpdatedName");
        root.GetProperty("surname").GetString().ShouldBe("UpdatedSurname");
        root.GetProperty("status").GetString().ShouldBe("ACTIVE");
    }

    [Fact]
    public async Task PutMe_UnauthenticatedUser_ReturnsUnauthorized()
    {
        // Arrange
        var request = new
        {
            Name = "UpdatedName",
            Surname = "UpdatedSurname"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/users/me", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PutMe_InvalidData_ReturnsBadRequest()
    {
        // Arrange
        const string email = "invalidputme@example.com";
        const string password = "Password123!";
        await SeedUser(email, password);
        var token = await GetAccessToken(email, password);

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var request = new
        {
            Name = "", // Empty name
            Surname = "UpdatedSurname"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/users/me", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PutMe_BannedUser_ReturnsUnauthorized()
    {
        // Arrange
        const string email = "banned_putme@example.com";
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

        var request = new
        {
            Name = "UpdatedName",
            Surname = "UpdatedSurname"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/users/me", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
