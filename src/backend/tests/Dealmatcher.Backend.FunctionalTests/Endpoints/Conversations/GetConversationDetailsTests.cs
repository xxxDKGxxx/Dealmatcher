namespace Dealmatcher.Backend.FunctionalTests.Endpoints.Conversations;

public class GetConversationDetailsTests(CustomWebApplicationFactory factory) : EndpointTestBase(factory)
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

    private async Task<int> CreateConversationInDb(string buyerEmail, int offerId)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var buyer = await db.Set<User>().FirstAsync(u => u.Email == buyerEmail);
        var offer = await db.Set<Offer>().Include(o => o.Seller).FirstAsync(o => o.Id == offerId);

        var conversation = new Conversation(offer, buyer);
        conversation.AddMessage(new Message(buyer, "Hello, is this available?"));

        db.Set<Conversation>().Add(conversation);
        await db.SaveChangesAsync();
        return conversation.Id;
    }

    [Fact]
    public async Task GetDetails_AsBuyer_ReturnsOk()
    {
        await RegisterAndLogin("seller@example.com", "Password123!");
        var buyerToken = await RegisterAndLogin("buyer@example.com", "Password123!");
        var offerId = await CreateOfferInDbAndReturnId("seller@example.com");
        var conversationId = await CreateConversationInDb("buyer@example.com", offerId);

        SetAuthHeader(buyerToken);
        var response = await _client.GetAsync($"/api/v1/conversations/{conversationId}");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        json.RootElement.GetProperty("id").GetInt32().ShouldBe(conversationId);
        json.RootElement.GetProperty("messages").GetArrayLength().ShouldBe(1);
    }

    [Fact]
    public async Task GetDetails_AsSeller_ReturnsOk()
    {
        var sellerToken = await RegisterAndLogin("seller2@example.com", "Password123!");
        await RegisterAndLogin("buyer2@example.com", "Password123!");
        var offerId = await CreateOfferInDbAndReturnId("seller2@example.com");
        var conversationId = await CreateConversationInDb("buyer2@example.com", offerId);

        SetAuthHeader(sellerToken);
        var response = await _client.GetAsync($"/api/v1/conversations/{conversationId}");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetDetails_Unauthenticated_ReturnsUnauthorized()
    {
        ClearAuthHeader();

        var response = await _client.GetAsync("/api/v1/conversations/1");

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetDetails_NotParticipant_ReturnsForbidden()
    {
        await RegisterAndLogin("seller3@example.com", "Password123!");
        await RegisterAndLogin("buyer3@example.com", "Password123!");
        var outsiderToken = await RegisterAndLogin("outsider@example.com", "Password123!");
        var offerId = await CreateOfferInDbAndReturnId("seller3@example.com");
        var conversationId = await CreateConversationInDb("buyer3@example.com", offerId);

        SetAuthHeader(outsiderToken);
        var response = await _client.GetAsync($"/api/v1/conversations/{conversationId}");

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetDetails_ConversationNotFound_ReturnsNotFound()
    {
        var token = await RegisterAndLogin("user@example.com", "Password123!");
        SetAuthHeader(token);

        var response = await _client.GetAsync("/api/v1/conversations/999");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
