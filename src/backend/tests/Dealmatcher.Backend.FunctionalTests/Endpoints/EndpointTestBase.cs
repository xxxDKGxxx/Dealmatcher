using Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate;
using Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.Categories;
using Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.Properties;
using FastEndpoints;

namespace Dealmatcher.Backend.FunctionalTests.Endpoints;

[Collection("Sequential")]
public abstract class EndpointTestBase(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    protected readonly HttpClient _client = factory.CreateClient();
    protected readonly CustomWebApplicationFactory _factory = factory;

    private static async Task CleanDatabaseAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
    }

    public async Task InitializeAsync()
    {
        await CleanDatabaseAsync(_factory.Services);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await SeedData.InitializeAsync(db);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    protected async Task SeedUser(string email, string password, string name = "Test", string surname = "User")
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var user = new User(email, hasher.HashPassword(password), name, surname);
        db.Set<User>().Add(user);
        await db.SaveChangesAsync();
    }

    protected async Task<string> RegisterAndLogin(string email, string password, string name = "Test", string surname = "User")
    {
        await _client.PostAsJsonAsync("/api/v1/users/register", new
        {
            Email = email,
            Password = password,
            Name = name,
            Surname = surname
        });

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/users/login", new
        {
            Email = email,
            Password = password
        });

        var json = await loginResponse.Content.ReadFromJsonAsync<JsonDocument>();
        return json!.RootElement.GetProperty("accessToken").GetString()!;
    }

    protected void SetAuthHeader(string token)
    {
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    protected void ClearAuthHeader()
    {
        _client.DefaultRequestHeaders.Authorization = null;
    }
}
