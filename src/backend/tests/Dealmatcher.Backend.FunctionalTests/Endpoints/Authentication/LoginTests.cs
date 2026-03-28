using System.Text.Json;

namespace Dealmatcher.Backend.FunctionalTests.Endpoints.Authentication;

public class LoginTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
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

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkWithToken()
    {
        await SeedUser("test@example.com", "Password123!");

        var response = await _client.PostAsJsonAsync("/api/v1/users/login", new
        {
            Email = "test@example.com",
            Password = "Password123!"
        });

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonDocument>();
        json.ShouldNotBeNull();

        var root = json.RootElement;
        root.GetProperty("accessToken").GetString().ShouldNotBeNullOrWhiteSpace();
        root.GetProperty("user").GetProperty("email").GetString().ShouldBe("test@example.com");
        root.GetProperty("user").GetProperty("name").GetString().ShouldBe("Test");
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsUnauthorized()
    {
        await SeedUser("wrong@example.com", "Password123!");

        var response = await _client.PostAsJsonAsync("/api/v1/users/login", new
        {
            Email = "wrong@example.com",
            Password = "WrongPassword!"
        });

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_NonexistentUser_ReturnsUnauthorized()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/users/login", new
        {
            Email = "nonexistent@example.com",
            Password = "Password123!"
        });

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_BannedUser_ReturnsForbidden()
    {
        await SeedUser("banned@example.com", "Password123!");

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var user = db.Set<User>().First(u => u.Email == "banned@example.com");
            user.BanUser();
            await db.SaveChangesAsync();
        }

        var response = await _client.PostAsJsonAsync("/api/v1/users/login", new
        {
            Email = "banned@example.com",
            Password = "Password123!"
        });

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
