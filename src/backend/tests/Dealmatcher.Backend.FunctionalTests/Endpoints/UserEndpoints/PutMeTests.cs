namespace Dealmatcher.Backend.FunctionalTests.Endpoints.UserEndpoints;

public class PutMeTests(CustomWebApplicationFactory factory) : EndpointTestBase(factory)
{
    [Fact]
    public async Task PutMe_AuthenticatedUser_ReturnsOkWithUpdatedProfile()
    {
        var token = await RegisterAndLogin("putme@example.com", "Password123!");
        SetAuthHeader(token);

        var response = await _client.PutAsJsonAsync("/api/v1/users/me", new
        {
            Name = "UpdatedName",
            Surname = "UpdatedSurname"
        });

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var user = await response.Content.ReadFromJsonAsync<JsonDocument>();
        user.ShouldNotBeNull();

        var root = user.RootElement;
        root.GetProperty("email").GetString().ShouldBe("putme@example.com");
        root.GetProperty("name").GetString().ShouldBe("UpdatedName");
        root.GetProperty("surname").GetString().ShouldBe("UpdatedSurname");
        root.GetProperty("status").GetString().ShouldBe("ACTIVE");
    }

    [Fact]
    public async Task PutMe_UnauthenticatedUser_ReturnsUnauthorized()
    {
        ClearAuthHeader();

        var response = await _client.PutAsJsonAsync("/api/v1/users/me", new
        {
            Name = "UpdatedName",
            Surname = "UpdatedSurname"
        });

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PutMe_InvalidData_ReturnsBadRequest()
    {
        var token = await RegisterAndLogin("invalidputme@example.com", "Password123!");
        SetAuthHeader(token);

        var response = await _client.PutAsJsonAsync("/api/v1/users/me", new
        {
            Name = "",
            Surname = "UpdatedSurname"
        });

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PutMe_BannedUser_ReturnsUnauthorized()
    {
        var token = await RegisterAndLogin("banned_putme@example.com", "Password123!");

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var user = db.Set<User>().First(u => u.Email == "banned_putme@example.com");
            user.BanUser("", 0, null);
            await db.SaveChangesAsync();
        }

        SetAuthHeader(token);

        var response = await _client.PutAsJsonAsync("/api/v1/users/me", new
        {
            Name = "UpdatedName",
            Surname = "UpdatedSurname"
        });

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
