using Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.PropertyDefinitions;

namespace Dealmatcher.Backend.FunctionalTests.Endpoints.UserEndpoints;

public class GetMeOffersTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _httpClient = factory.CreateClient();
    private readonly CustomWebApplicationFactory _factory = factory;
    private static readonly string[] value = new[] { "test" };

    private async Task<string> RegisterAndLogin(string email, string password, string name = "Test", string surname = "User")
    {
        await _httpClient.PostAsJsonAsync("/api/v1/users/register", new
        {
            Email = email,
            Password = password,
            Name = name,
            Surname = surname
        });

        var loginResponse = await _httpClient.PostAsJsonAsync("/api/v1/users/login", new
        {
            Email = email,
            Password = password
        });

        var json = await loginResponse.Content.ReadFromJsonAsync<JsonDocument>();
        return json!.RootElement.GetProperty("accessToken").GetString()!;
    }

    private void SetAuthHeader(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    private async Task SeedCategoryWithDefinitions()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (await db.Set<Category>().AnyAsync()) return;

        var category = new Category("Samochody", "Kategoria samochodów");
        category.AddPropertyDefinition(new NumericPropertyDefinition("Przebieg", PropertyType.Numeric));
        category.AddPropertyDefinition(new BooleanPropertyDefinition("Uszkodzony", PropertyType.Boolean));
        db.Set<Category>().Add(category);
        await db.SaveChangesAsync();
    }

    private async Task CreateOffer(string token, string title, int categoryId, Dictionary<string, string> properties)
    {
        SetAuthHeader(token);
        await _httpClient.PostAsJsonAsync("/api/v1/offers", new
        {
            Title = title,
            Description = "Opis oferty",
            Price = 10000,
            Tags = value,
            CategoryId = categoryId,
            Properties = properties,
            Availability = 1
        });
    }

    [Fact]
    public async Task GetMeOffers_Unauthenticated_ReturnsUnauthorized()
    {
        _httpClient.DefaultRequestHeaders.Authorization = null;

        var response = await _httpClient.GetAsync("/api/v1/users/me/offers");

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMeOffers_AuthenticatedUserNoOffers_ReturnsOkWithEmptyList()
    {
        var token = await RegisterAndLogin("nooffers@example.com", "Password123!");
        SetAuthHeader(token);

        var response = await _httpClient.GetAsync("/api/v1/users/me/offers");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        json.RootElement.GetArrayLength().ShouldBe(0);
    }

    [Fact]
    public async Task GetMeOffers_AuthenticatedUserWithOffers_ReturnsOkWithOffers()
    {
        await SeedCategoryWithDefinitions();

        var token = await RegisterAndLogin("withoffers@example.com", "Password123!");

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var category = await db.Set<Category>().Include(c => c.PropertyDefinitions).FirstAsync();
            var przebiegDef = category.PropertyDefinitions.First(pd => pd.Name == "Przebieg");
            var uszkodzonyDef = category.PropertyDefinitions.First(pd => pd.Name == "Uszkodzony");

            await CreateOffer(token, "BMW E46", category.Id, new Dictionary<string, string>
            {
                [przebiegDef.Id.ToString()] = "180000",
                [uszkodzonyDef.Id.ToString()] = "false"
            });

            await CreateOffer(token, "Audi A4", category.Id, new Dictionary<string, string>
            {
                [przebiegDef.Id.ToString()] = "120000",
                [uszkodzonyDef.Id.ToString()] = "true"
            });
        }

        SetAuthHeader(token);
        var response = await _httpClient.GetAsync("/api/v1/users/me/offers");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        json.RootElement.GetArrayLength().ShouldBe(2);
    }

    [Fact]
    public async Task GetMeOffers_AuthenticatedUser_DoesNotReturnOtherUsersOffers()
    {
        await SeedCategoryWithDefinitions();

        var tokenUser1 = await RegisterAndLogin("user1offers@example.com", "Password123!");
        var tokenUser2 = await RegisterAndLogin("user2offers@example.com", "Password123!");

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var category = await db.Set<Category>().Include(c => c.PropertyDefinitions).FirstAsync();
            var przebiegDef = category.PropertyDefinitions.First(pd => pd.Name == "Przebieg");

            await CreateOffer(tokenUser1, "Oferta usera 1", category.Id, new Dictionary<string, string>
            {
                [przebiegDef.Id.ToString()] = "50000"
            });
        }

        SetAuthHeader(tokenUser2);
        var response = await _httpClient.GetAsync("/api/v1/users/me/offers");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        json.RootElement.GetArrayLength().ShouldBe(0);
    }

    [Fact]
    public async Task GetMeOffers_BannedUser_ReturnsUnauthorized()
    {
        var token = await RegisterAndLogin("bannedoffers@example.com", "Password123!");

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var user = await db.Set<User>().FirstAsync(u => u.Email == "bannedoffers@example.com");
            user.BanUser();
            await db.SaveChangesAsync();
        }

        SetAuthHeader(token);
        var response = await _httpClient.GetAsync("/api/v1/users/me/offers");

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
