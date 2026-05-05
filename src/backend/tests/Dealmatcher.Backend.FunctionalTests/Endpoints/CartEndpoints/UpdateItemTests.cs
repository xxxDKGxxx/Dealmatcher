namespace Dealmatcher.Backend.FunctionalTests.Endpoints.CartEndpoints;

public class UpdateItemTests(CustomWebApplicationFactory factory) : EndpointTestBase(factory)
{
    private async Task<int> SeedOfferAndCart(string buyerEmail, int offerAvailability = 5, bool addToCart = true, int initialQuantity = 1)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var buyer = db.Set<User>().First(u => u.Email == buyerEmail);

        var seller = new User(
            $"seller_update_{Guid.NewGuid()}@example.com",
            "hashed_password",
            "Seller",
            "User"
        );
        db.Set<User>().Add(seller);

        var category = db.Set<Category>().FirstOrDefault(c => c.Name == "Cars");
        if (category == null)
        {
            category = new Category("Cars", "Description");
            db.Set<Category>().Add(category);
        }

        var offer = new OfferEntity(
            "Test Update Offer",
            "Test Description",
            100,
            ["image1.jpg"],
            seller,
            ["tag1"],
            offerAvailability,
            category,
            []
        );
        db.Set<OfferEntity>().Add(offer);
        await db.SaveChangesAsync();

        if (addToCart)
        {
            var cartRepository = scope.ServiceProvider.GetRequiredService<ICartRepository>();
            var cart = await cartRepository.GetCartAsync(buyer.Id, CancellationToken.None);

            cart.UpdateItemQuantity(offer.Id, initialQuantity);
            await cartRepository.SaveCartAsync(cart, CancellationToken.None);
        }

        return offer.Id;
    }

    [Fact]
    public async Task UpdateItem_ValidData_ReturnsOk()
    {
        // Arrange
        var email = $"buyer_update_{Guid.NewGuid()}@example.com";
        var token = await RegisterAndLogin(email, "Password123!");
        SetAuthHeader(token);

        var offerId = await SeedOfferAndCart(email, offerAvailability: 10, addToCart: true, initialQuantity: 1);

        var requestBody = new { Quantity = 5 };

        // Act
        var response = await _client.PatchAsJsonAsync($"/api/v1/cart/items/{offerId}", requestBody);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var cartItem = await response.Content.ReadFromJsonAsync<CartItemDto>();
        cartItem.ShouldNotBeNull();
        cartItem.Quantity.ShouldBe(5);
        cartItem.Offer.Id.ShouldBe(offerId);
    }

    [Fact]
    public async Task UpdateItem_Unauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthHeader();
        var requestBody = new { Quantity = 2 };

        // Act
        var response = await _client.PatchAsJsonAsync("/api/v1/cart/items/1", requestBody);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateItem_ItemNotInCart_ReturnsNotFound()
    {
        // Arrange
        var email = $"buyer_notincart_{Guid.NewGuid()}@example.com";
        var token = await RegisterAndLogin(email, "Password123!");
        SetAuthHeader(token);

        var offerId = await SeedOfferAndCart(email, offerAvailability: 5, addToCart: false);

        var requestBody = new { Quantity = 2 };

        // Act
        var response = await _client.PatchAsJsonAsync($"/api/v1/cart/items/{offerId}", requestBody);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateItem_OfferDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var email = $"buyer_notfound_{Guid.NewGuid()}@example.com";
        var token = await RegisterAndLogin(email, "Password123!");
        SetAuthHeader(token);

        var requestBody = new { Quantity = 2 };

        // Act
        var response = await _client.PatchAsJsonAsync("/api/v1/cart/items/99999", requestBody);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateItem_QuantityExceedsAvailability_ReturnsBadRequest()
    {
        // Arrange
        var email = $"buyer_exceeds_{Guid.NewGuid()}@example.com";
        var token = await RegisterAndLogin(email, "Password123!");
        SetAuthHeader(token);

        var offerId = await SeedOfferAndCart(email, offerAvailability: 2, addToCart: true, initialQuantity: 1);

        var requestBody = new { Quantity = 5 };

        // Act
        var response = await _client.PatchAsJsonAsync($"/api/v1/cart/items/{offerId}", requestBody);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateItem_QuantityLessThanOne_ReturnsBadRequest()
    {
        // Arrange
        var email = $"buyer_zero_{Guid.NewGuid()}@example.com";
        var token = await RegisterAndLogin(email, "Password123!");
        SetAuthHeader(token);

        var offerId = await SeedOfferAndCart(email, offerAvailability: 10, addToCart: true);

        var requestBody = new { Quantity = 0 };

        // Act
        var response = await _client.PatchAsJsonAsync($"/api/v1/cart/items/{offerId}", requestBody);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
