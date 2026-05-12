namespace Dealmatcher.Backend.FunctionalTests.Endpoints.Conversations;

public class SendMessageTests(CustomWebApplicationFactory factory) : EndpointTestBase(factory)
{
    private async Task<int> SeedConversation(string sellerEmail, string buyerEmail)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var seller = db.Set<User>().First(u => u.Email == sellerEmail);
        var buyer = db.Set<User>().First(u => u.Email == buyerEmail);
        var category = db.Set<Category>().First();

        var offer = new Offer(
            "Test Offer For Messages",
            "Description",
            100m,
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
    public async Task SendMessage_ValidData_ReturnsCreated()
    {
        // Arrange
        await RegisterAndLogin("seller_msg@example.com", "Password123!");
        var buyerToken = await RegisterAndLogin("buyer_msg@example.com", "Password123!");

        var conversationId = await SeedConversation("seller_msg@example.com", "buyer_msg@example.com");

        SetAuthHeader(buyerToken);

        var requestBody = new { Content = "Hello, is this still available?" };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1/conversations/{conversationId}/messages", requestBody);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);

        json.RootElement.GetProperty("id").GetInt32().ShouldBeGreaterThan(0);
        json.RootElement.GetProperty("content").GetString().ShouldBe("Hello, is this still available?");
        json.RootElement.GetProperty("status").GetString().ShouldBe("SENT");
    }

    [Fact]
    public async Task SendMessage_SellerReplies_ReturnsCreated()
    {
        // Arrange
        var sellerToken = await RegisterAndLogin("seller_reply@example.com", "Password123!");
        await RegisterAndLogin("buyer_reply@example.com", "Password123!");

        var conversationId = await SeedConversation("seller_reply@example.com", "buyer_reply@example.com");

        SetAuthHeader(sellerToken);

        var requestBody = new { Content = "Yes, it is!" };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1/conversations/{conversationId}/messages", requestBody);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        json.RootElement.GetProperty("content").GetString().ShouldBe("Yes, it is!");
    }

    [Fact]
    public async Task SendMessage_Unauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthHeader();
        var requestBody = new { Content = "Hello" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/conversations/1/messages", requestBody);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SendMessage_ConversationNotFound_ReturnsNotFound()
    {
        // Arrange
        var token = await RegisterAndLogin("buyer_notfound@example.com", "Password123!");
        SetAuthHeader(token);

        var requestBody = new { Content = "Hello" };

        var response = await _client.PostAsJsonAsync("/api/v1/conversations/9999/messages", requestBody);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SendMessage_EmptyMessage_ReturnsBadRequest()
    {
        // Arrange
        await RegisterAndLogin("seller_empty@example.com", "Password123!");
        var buyerToken = await RegisterAndLogin("buyer_empty@example.com", "Password123!");

        var conversationId = await SeedConversation("seller_empty@example.com", "buyer_empty@example.com");

        SetAuthHeader(buyerToken);
        var requestBody = new { Content = "   " };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1/conversations/{conversationId}/messages", requestBody);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SendMessage_UserNotParticipant_ReturnsForbidden()
    {
        // Arrange
        await RegisterAndLogin("seller_part@example.com", "Password123!");
        await RegisterAndLogin("buyer_part@example.com", "Password123!");

        var strangerToken = await RegisterAndLogin("stranger@example.com", "Password123!");

        var conversationId = await SeedConversation("seller_part@example.com", "buyer_part@example.com");

        SetAuthHeader(strangerToken);
        var requestBody = new { Content = "I want to buy it too!" };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1/conversations/{conversationId}/messages", requestBody);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task SendMessage_ConversationClosed_ReturnsConflict()
    {
        // Arrange
        await RegisterAndLogin("seller_closed@example.com", "Password123!");
        var buyerToken = await RegisterAndLogin("buyer_closed@example.com", "Password123!");

        var conversationId = await SeedConversation("seller_closed@example.com", "buyer_closed@example.com");

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var conv = db.Set<Conversation>().First(c => c.Id == conversationId);

            var statusProperty = typeof(Conversation).GetProperty("Status");
            statusProperty?.SetValue(conv, ConversationStatus.Closed);

            await db.SaveChangesAsync();
        }

        SetAuthHeader(buyerToken);
        var requestBody = new { Content = "Wait, please reopen!" };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1/conversations/{conversationId}/messages", requestBody);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }
}
