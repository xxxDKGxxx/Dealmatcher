namespace Dealmatcher.Backend.FunctionalTests.Endpoints.Conversations;

public class GetConversationsTests(CustomWebApplicationFactory factory) : EndpointTestBase(factory)
{
    private async Task<int> SeedConversation(string sellerEmail, string buyerEmail)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var seller = db.Set<User>().First(u => u.Email == sellerEmail);
        var buyer = db.Set<User>().First(u => u.Email == buyerEmail);
        var category = db.Set<Category>().First();

        var offer = new Offer(
            "List Test Offer",
            "Description",
            500m,
            [],
            seller,
            [],
            1,
            category,
            []
        );
        db.Set<Offer>().Add(offer);

        var conversation = new Conversation(offer, buyer);

        var message = new Message(buyer, "Initial message");
        conversation.AddMessage(message);

        db.Set<Conversation>().Add(conversation);

        await db.SaveChangesAsync();

        return conversation.Id;
    }

    [Fact]
    public async Task GetConversations_AuthenticatedUser_ReturnsOkWithConversations()
    {
        // Arrange
        await RegisterAndLogin("seller_list@example.com", "Password123!");
        var buyerToken = await RegisterAndLogin("buyer_list@example.com", "Password123!");

        var conversationId = await SeedConversation("seller_list@example.com", "buyer_list@example.com");

        SetAuthHeader(buyerToken);

        // Act
        var response = await _client.GetAsync("/api/v1/conversations");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var conversations = await response.Content.ReadFromJsonAsync<List<ConversationDto>>();
        conversations.ShouldNotBeNull();
        conversations.ShouldNotBeEmpty();
        conversations.Any(c => c.Id == conversationId).ShouldBeTrue();
    }

    [Fact]
    public async Task GetConversations_UnauthenticatedUser_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthHeader();

        // Act
        var response = await _client.GetAsync("/api/v1/conversations");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetConversations_UserWithNoConversations_ReturnsOkWithEmptyList()
    {
        // Arrange
        var token = await RegisterAndLogin("lonely_user@example.com", "Password123!");
        SetAuthHeader(token);

        // Act
        var response = await _client.GetAsync("/api/v1/conversations");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var conversations = await response.Content.ReadFromJsonAsync<List<ConversationDto>>();
        conversations.ShouldBeEmpty();
    }
}
