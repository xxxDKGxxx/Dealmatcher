using Dealmatcher.Backend.Domain.Core.Cart.Dto;

namespace Dealmatcher.Backend.FunctionalTests.Endpoints.CartEndpoints;

public class AddItemTests(CustomWebApplicationFactory factory) : EndpointTestBase(factory)
{
    private async Task<int> SeedOffer(int availability = 5)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var user = new User(
            $"seller_cart_{Guid.NewGuid()}@example.com",
            passwordHasher.HashPassword("Password123!"),
            "Seller",
            "User"
        );
        db.Set<User>().Add(user);

        var category = db.Set<Category>().First(c => c.Name == "Cars");

        var offer = new OfferEntity(
            "Test Offer For Cart",
            "Test Description",
            100,
            ["image1.jpg"],
            user,
            ["tag1"],
            availability,
            category,
            []
        );

        db.Set<OfferEntity>().Add(offer);

        await db.SaveChangesAsync();

        return offer.Id;
    }

    private async Task<int> SeedUser(int userId)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var user = db.Set<User>().Find(userId)!;
        var category = db.Set<Category>().First(c => c.Name == "Cars");

        var offer = new OfferEntity(
            "My Own Offer",
            "Test Description",
            100,
            ["image1.jpg"],
            user,
            ["tag1"],
            5,
            category,
            []
        );

        db.Set<OfferEntity>().Add(offer);

        await db.SaveChangesAsync();

        return offer.Id;
    }

    [Fact]
    public async Task AddItem_ValidData_ReturnsCreated()
    {
        // Arrange
        var token = await RegisterAndLogin($"buyer_{Guid.NewGuid()}@example.com", "Password123!");
        SetAuthHeader(token);

        var offerId = await SeedOffer();
        var request = new { OfferId = offerId, Quantity = 2 };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/cart/items", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var cartItem = await response.Content.ReadFromJsonAsync<CartItemDto>();
        cartItem.ShouldNotBeNull();
        cartItem.Quantity.ShouldBe(2);
        cartItem.Offer.Id.ShouldBe(offerId);
    }

    [Fact]
    public async Task AddItem_Unauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthHeader();
        var request = new { OfferId = 1, Quantity = 1 };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/cart/items", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AddItem_InvalidData_ReturnsBadRequest()
    {
        // Arrange
        var token = await RegisterAndLogin($"buyer_{Guid.NewGuid()}@example.com", "Password123!");
        SetAuthHeader(token);

        var request = new { OfferId = -1, Quantity = 0 };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/cart/items", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddItem_NonExistingOffer_ReturnsNotFound()
    {
        // Arrange
        var token = await RegisterAndLogin($"buyer_{Guid.NewGuid()}@example.com", "Password123!");
        SetAuthHeader(token);

        var request = new { OfferId = 99999, Quantity = 1 };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/cart/items", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddItem_AlreadyInCart_ReturnsConflict()
    {
        // Arrange
        var token = await RegisterAndLogin($"buyer_{Guid.NewGuid()}@example.com", "Password123!");
        SetAuthHeader(token);

        var offerId = await SeedOffer();
        var request = new { OfferId = offerId, Quantity = 1 };

        // Act
        var firstResponse = await _client.PostAsJsonAsync("/api/v1/cart/items", request);
        firstResponse.StatusCode.ShouldBe(HttpStatusCode.Created);

        var secondResponse = await _client.PostAsJsonAsync("/api/v1/cart/items", request);

        // Assert
        secondResponse.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task AddItem_OwnOffer_ReturnsConflict()
    {
        // Arrange
        var email = $"seller_{Guid.NewGuid()}@example.com";
        var token = await RegisterAndLogin(email, "Password123!");
        SetAuthHeader(token);

        int userId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            userId = db.Set<User>().Single(u => u.Email == email).Id;
        }

        var offerId = await SeedUser(userId);
        var request = new { OfferId = offerId, Quantity = 1 };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/cart/items", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }
}
