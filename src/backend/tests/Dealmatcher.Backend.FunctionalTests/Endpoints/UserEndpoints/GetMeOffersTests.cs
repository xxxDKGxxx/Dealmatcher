using AutoMapper;
using Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.Properties;

namespace Dealmatcher.Backend.FunctionalTests.Endpoints.UserEndpoints;

public class GetMeOffersTests(CustomWebApplicationFactory factory) : EndpointTestBase(factory)
{
    private static readonly string[] _value = ["test"];

    private async Task CreateOfferInDb(string sellerEmail, string title, Dictionary<string, string> propertyValues)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var seller = await db.Set<User>().FirstAsync(u => u.Email == sellerEmail);
        var category = await db.Set<Category>().Include(c => c.PropertyDefinitions).FirstAsync();

        List<Property> properties = [];
        foreach (var (propId, value) in propertyValues)
        {
            var definition = category.PropertyDefinitions.First(pd => pd.Id == int.Parse(propId));
            properties.Add(definition.CreatePropertyFromString(value));
        }

        var offer = new OfferEntity(
            title,
            "Test description",
            10000m,
            [],
            seller,
            ["test"],
            1,
            category,
            properties);

        db.Set<OfferEntity>().Add(offer);
        await db.SaveChangesAsync();
    }

    [Fact]
    public async Task GetMeOffers_AuthenticatedUserNoOffers_ReturnsNoContent()
    {
        var token = await RegisterAndLogin("nooffers@example.com", "Password123!");
        SetAuthHeader(token);

        var response = await _client.GetAsync("/api/v1/users/me/offers");

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GetMeOffers_AuthenticatedUserWithOffers_ReturnsOkWithOffers()
    {
        var token = await RegisterAndLogin("withoffers@example.com", "Password123!");

        int mileageDefId;
        int damagedDefId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var category = await db.Set<Category>().Include(c => c.PropertyDefinitions).FirstAsync();
            mileageDefId = category.PropertyDefinitions.First(pd => pd.Name == "Mileage").Id;
            damagedDefId = category.PropertyDefinitions.First(pd => pd.Name == "Damaged").Id;
        }

        await CreateOfferInDb("withoffers@example.com", "BMW E46", new Dictionary<string, string>
        {
            [mileageDefId.ToString()] = "180000",
            [damagedDefId.ToString()] = "false"
        });

        await CreateOfferInDb("withoffers@example.com", "Audi A4", new Dictionary<string, string>
        {
            [mileageDefId.ToString()] = "120000",
            [damagedDefId.ToString()] = "true"
        });

        SetAuthHeader(token);
        var response = await _client.GetAsync("/api/v1/users/me/offers");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        json.RootElement.GetArrayLength().ShouldBe(2);
    }

    [Fact]
    public async Task GetMeOffers_AuthenticatedUser_DoesNotReturnOtherUsersOffers()
    {
        var tokenUser1 = await RegisterAndLogin("user1offers@example.com", "Password123!");
        var tokenUser2 = await RegisterAndLogin("user2offers@example.com", "Password123!");

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var category = await db.Set<Category>().Include(c => c.PropertyDefinitions).FirstAsync();
            var mileageDef = category.PropertyDefinitions.First(pd => pd.Name == "Mileage");

            await CreateOfferInDb("user1offers@example.com", "User1 offer", new Dictionary<string, string>
            {
                [mileageDef.Id.ToString()] = "50000"
            });
        }

        SetAuthHeader(tokenUser2);
        var response = await _client.GetAsync("/api/v1/users/me/offers");

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
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
        var response = await _client.GetAsync("/api/v1/users/me/offers");

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
