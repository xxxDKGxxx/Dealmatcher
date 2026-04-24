namespace Dealmatcher.Backend.FunctionalTests.Endpoints.OfferEndpoints;

public class CreateTests(CustomWebApplicationFactory factory) : EndpointTestBase(factory)
{
    private async Task<int> CreateCategory()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var category = new Category(
            "Test Category",
            "Test category description");

        db.Set<Category>().Add(category);
        await db.SaveChangesAsync();

        return category.Id;
    }

    [Fact]
    public async Task CreateOffer_ValidDataWithImage_ReturnsCreated()
    {
        var token = await RegisterAndLogin("seller@example.com", "Password123!");
        SetAuthHeader(token);

        var categoryId = await CreateCategory();

        using var formData = new MultipartFormDataContent
        {
            { new StringContent("Super Oferta"), "Title" },
            { new StringContent("Bardzo fajny opis produktu"), "Description" },
            { new StringContent(99.99m.ToString()), "Price" },
            { new StringContent(categoryId.ToString()), "CategoryId" },
            { new StringContent("5"), "Availability" },
            { new StringContent("promocja"), "Tags" },
            { new StringContent("nowosc"), "Tags" }
        };

        var imageBytes = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        var imageContent = new ByteArrayContent(imageBytes);
        imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        formData.Add(imageContent, "Images", "test-image.png");

        var response = await _client.PostAsync("/api/v1/offers", formData);

        if (response.StatusCode != HttpStatusCode.Created)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Expected 201 but got {response.StatusCode}: {error}");
        }

        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        var json = await response.Content.ReadFromJsonAsync<OfferDto>();
        json.ShouldNotBeNull();
        json.Title.ShouldBe("Super Oferta");
        json.Price.ShouldBe(99.99m);
        json.Images.Count.ShouldBe(1);
    }

    [Fact]
    public async Task CreateOffer_UnauthenticatedUser_ReturnsUnauthorized()
    {
        using var formData = new MultipartFormDataContent
        {
            { new StringContent("Super Oferta"), "Title" },
            { new StringContent("Opis"), "Description" },
            { new StringContent("99.99"), "Price" },
            { new StringContent("1"), "CategoryId" }
        };

        var response = await _client.PostAsync("/api/v1/offers", formData);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateOffer_InvalidData_ReturnsBadRequest()
    {
        var token = await RegisterAndLogin("badrequest@example.com", "Password123!");
        SetAuthHeader(token);

        var categoryId = await CreateCategory();

        using var formData = new MultipartFormDataContent
        {
            { new StringContent(""), "Title" },
            { new StringContent("Opis"), "Description" },
            { new StringContent("-50"), "Price" },
            { new StringContent(categoryId.ToString()), "CategoryId" }
        };

        var response = await _client.PostAsync("/api/v1/offers", formData);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
