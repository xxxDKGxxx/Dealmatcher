namespace Dealmatcher.Backend.FunctionalTests.Endpoints.Authentication;

public class RegisterTests(CustomWebApplicationFactory factory) : EndpointTestBase(factory)
{
    [Fact]
    public async Task Register_ValidData_ReturnsCreated()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/users/register", new
        {
            Email = "newuser@example.com",
            Password = "ValidPassword123",
            Name = "Jan",
            Surname = "Kowalski"
        });

        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        var json = await response.Content.ReadFromJsonAsync<JsonDocument>();
        json.ShouldNotBeNull();

        var root = json.RootElement;
        root.GetProperty("email").GetString().ShouldBe("newuser@example.com");
        root.GetProperty("name").GetString().ShouldBe("Jan");
        root.GetProperty("status").GetString().ShouldBe(UserStatus.Active.Name);
    }

    [Fact]
    public async Task Register_InvalidData_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/users/register", new
        {
            Email = "bad-email-format",
            Password = "123",
            Name = "",
            Surname = "Kowalski"
        });

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_EmailTakenByActiveUser_ReturnsConflict()
    {
        await SeedUser("active@example.com", "Password123!");

        var response = await _client.PostAsJsonAsync("/api/v1/users/register", new
        {
            Email = "active@example.com",
            Password = "ValidPassword123",
            Name = "Jan",
            Surname = "Kowalski"
        });

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Register_EmailTakenByBannedUser_ReturnsConflict()
    {
        await SeedUser("banned@example.com", "Password123!");

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var user = db.Set<User>().First(u => u.Email == "banned@example.com");
            user.BanUser("", 0, null);
            await db.SaveChangesAsync();
        }

        var response = await _client.PostAsJsonAsync("/api/v1/users/register", new
        {
            Email = "banned@example.com",
            Password = "ValidPassword123",
            Name = "Jan",
            Surname = "Kowalski"
        });

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Register_EmailTakenByInactiveUser_ReturnsCreated()
    {
        await SeedUser("inactive@example.com", "Password123!");

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var user = db.Set<User>().First(u => u.Email == "inactive@example.com");
            user.DeactivateUserAccount();
            await db.SaveChangesAsync();
        }

        var response = await _client.PostAsJsonAsync("/api/v1/users/register", new
        {
            Email = "inactive@example.com",
            Password = "ValidPassword123",
            Name = "NowyJan",
            Surname = "NowyKowalski"
        });

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }
}
