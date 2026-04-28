using Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.Properties;

namespace Dealmatcher.Backend.FunctionalTests.Endpoints.Conversations;

public class CreateConversationTests(CustomWebApplicationFactory factory) : EndpointTestBase(factory)
{
    private async Task<int> CreateOfferInDbAndReturnId(string sellerEmail)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var seller = await db.Set<User>().FirstAsync(u => u.Email == sellerEmail);
        var category = await db.Set<Category>().Include(c => c.PropertyDefinitions).FirstAsync();
        var mileageDef = category.PropertyDefinitions.First(pd => pd.Name == "Mileage");
        var damagedDef = category.PropertyDefinitions.First(pd => pd.Name == "Damaged");

        List<Property> properties =
        [
            mileageDef.CreatePropertyFromString("120000"),
            damagedDef.CreatePropertyFromString("false")
        ];

        var offer = new Offer("Test Offer", "Description", 10000m, [], seller, [], 1, category, properties);
        db.Set<Offer>().Add(offer);
        await db.SaveChangesAsync();
        return offer.Id;
    }

    [Fact]
    public async Task Create_ValidData_ReturnsCreated()
    {
        await RegisterAndLogin("seller@example.com", "Password123!");
        var buyerToken = await RegisterAndLogin("buyer@example.com", "Password123!");
        var offerId = await CreateOfferInDbAndReturnId("seller@example.com");

        SetAuthHeader(buyerToken);
        var response = await _client.PostAsJsonAsync("/api/v1/conversations", new
        {
            OfferId = offerId,
            InitialMessage = "Is this still available?"
        });

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        json.RootElement.GetProperty("id").GetInt32().ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task Create_Unauthenticated_ReturnsUnauthorized()
    {
        ClearAuthHeader();

        var response = await _client.PostAsJsonAsync("/api/v1/conversations", new
        {
            OfferId = 1,
            InitialMessage = "Hello"
        });

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_OfferNotFound_ReturnsNotFound()
    {
        var token = await RegisterAndLogin("buyer2@example.com", "Password123!");
        SetAuthHeader(token);

        var response = await _client.PostAsJsonAsync("/api/v1/conversations", new
        {
            OfferId = 999,
            InitialMessage = "Hello"
        });

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_BuyerIsSeller_ReturnsForbidden()
    {
        var sellerToken = await RegisterAndLogin("sellerbuyer@example.com", "Password123!");
        var offerId = await CreateOfferInDbAndReturnId("sellerbuyer@example.com");

        SetAuthHeader(sellerToken);
        var response = await _client.PostAsJsonAsync("/api/v1/conversations", new
        {
            OfferId = offerId,
            InitialMessage = "Can I buy my own stuff?"
        });

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Create_EmptyMessage_ReturnsBadRequest()
    {
        await RegisterAndLogin("seller3@example.com", "Password123!");
        var buyerToken = await RegisterAndLogin("buyer3@example.com", "Password123!");
        var offerId = await CreateOfferInDbAndReturnId("seller3@example.com");

        SetAuthHeader(buyerToken);
        var response = await _client.PostAsJsonAsync("/api/v1/conversations", new
        {
            OfferId = offerId,
            InitialMessage = ""
        });

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_DuplicateConversation_ReturnsConflict()
    {
        await RegisterAndLogin("seller4@example.com", "Password123!");
        var buyerToken = await RegisterAndLogin("buyer4@example.com", "Password123!");
        var offerId = await CreateOfferInDbAndReturnId("seller4@example.com");

        SetAuthHeader(buyerToken);
        await _client.PostAsJsonAsync("/api/v1/conversations", new
        {
            OfferId = offerId,
            InitialMessage = "First message"
        });

        var response = await _client.PostAsJsonAsync("/api/v1/conversations", new
        {
            OfferId = offerId,
            InitialMessage = "Second attempt"
        });

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }
}
