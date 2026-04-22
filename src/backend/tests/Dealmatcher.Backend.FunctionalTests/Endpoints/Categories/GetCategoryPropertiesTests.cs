namespace Dealmatcher.Backend.FunctionalTests.Endpoints.Categories;

public class GetCategoryPropertiesTests(CustomWebApplicationFactory factory) : EndpointTestBase(factory)
{
    [Fact]
    public async Task GetCategoryProperties_ExistingCategory_ReturnsOkWithProperties()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/categories/Cars/properties");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var properties = await response.Content.ReadFromJsonAsync<List<PropertyDefinitionDto>>();
        properties.ShouldNotBeNull();
        properties.ShouldNotBeEmpty();

        // Assert properties exist based on SeedData for "Cars"
        properties.ShouldContain(p => p.Name == "Brand" && p.Type == "Select" && p.options != null && p.options.Contains("BMW"));
        properties.ShouldContain(p => p.Name == "Mileage" && p.Type == "Numeric");
        properties.ShouldContain(p => p.Name == "Year of production" && p.Type == "Numeric");
        properties.ShouldContain(p => p.Name == "Damaged" && p.Type == "Boolean");
    }

    [Fact]
    public async Task GetCategoryProperties_NonExistingCategory_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/categories/NonExistingCategory/properties");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
