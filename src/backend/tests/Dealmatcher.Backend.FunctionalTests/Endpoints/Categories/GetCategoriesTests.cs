namespace Dealmatcher.Backend.FunctionalTests.Endpoints.Categories;

public class GetCategoriesTests(CustomWebApplicationFactory factory) : EndpointTestBase(factory)
{
    [Fact]
    public async Task GetCategories_ReturnsOkWithAllCategories()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/categories");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var categories = await response.Content.ReadFromJsonAsync<List<CategoryDto>>();
        categories.ShouldNotBeNull();
        categories.Count.ShouldBeGreaterThanOrEqualTo(3); // Based on SeedData
        categories.ShouldContain(c => c.Name == "Cars");
        categories.ShouldContain(c => c.Name == "Phones");
        categories.ShouldContain(c => c.Name == "Clothing");
    }
}
