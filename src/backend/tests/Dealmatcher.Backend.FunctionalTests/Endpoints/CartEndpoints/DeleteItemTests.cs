namespace Dealmatcher.Backend.FunctionalTests.Endpoints.CartEndpoints;

public class DeleteItemTests(CustomWebApplicationFactory factory) : EndpointTestBase(factory)
{
    private async Task<int> SeedOfferAndCart(string buyerEmail, int offerAvailability = 5, bool addToCart = true, int initialQuantity = 1)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var buyer = db.Set<User>().First(u => u.Email == buyerEmail);

        var seller = new User(
            $"seller_delete_{Guid.NewGuid()}@example.com",
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
            "Test Delete Offer",
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
    public async Task DeleteItem_ItemInCart_ReturnsNoContent()
    {
        // Arrange
        var email = $"buyer_delete_{Guid.NewGuid()}@example.com";
        var token = await RegisterAndLogin(email, "Password123!");
        SetAuthHeader(token);

        var offerId = await SeedOfferAndCart(email, addToCart: true);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/cart/items/{offerId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        // Verify it was removed
        var getCartResponse = await _client.GetAsync("/api/v1/cart/items");
        var cartItems = await getCartResponse.Content.ReadFromJsonAsync<List<CartItemDto>>();
        cartItems.ShouldNotBeNull();
        cartItems.ShouldNotContain(ci => ci.Offer.Id == offerId);
    }

    [Fact]
    public async Task DeleteItem_Unauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthHeader();

        // Act
        var response = await _client.DeleteAsync("/api/v1/cart/items/1");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteItem_ItemNotInCart_ReturnsNotFound()
    {
        // Arrange
        var email = $"buyer_delete_notin_{Guid.NewGuid()}@example.com";
        var token = await RegisterAndLogin(email, "Password123!");
        SetAuthHeader(token);

        var offerId = await SeedOfferAndCart(email, addToCart: false);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/cart/items/{offerId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
