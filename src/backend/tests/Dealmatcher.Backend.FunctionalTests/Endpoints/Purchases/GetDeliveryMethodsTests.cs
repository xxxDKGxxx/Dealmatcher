namespace Dealmatcher.Backend.FunctionalTests.Endpoints.Purchases;

public class GetDeliveryMethodsTests(CustomWebApplicationFactory factory) : EndpointTestBase(factory)
{
    [Fact]
    public async Task GetDeliveryMethods_ReturnsOkWithDeliveryMethods()
    {
        ClearAuthHeader();

        var response = await _client.GetAsync("/api/v1/purchases/delivery-methods");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        json.RootElement.GetArrayLength().ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task GetDeliveryMethods_ReturnsCorrectStructure()
    {
        ClearAuthHeader();

        var response = await _client.GetAsync("/api/v1/purchases/delivery-methods");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);

        json.RootElement.GetArrayLength().ShouldBeGreaterThan(0);
        var firstMethod = json.RootElement[0];

        firstMethod.TryGetProperty("id", out _).ShouldBeTrue();
        firstMethod.TryGetProperty("name", out _).ShouldBeTrue();
        firstMethod.TryGetProperty("description", out _).ShouldBeTrue();
        firstMethod.TryGetProperty("price", out _).ShouldBeTrue();
        firstMethod.TryGetProperty("estimatedDays", out _).ShouldBeTrue();
    }

    [Fact]
    public async Task GetDeliveryMethods_IsAnonymous_ReturnsOk()
    {
        ClearAuthHeader();

        var response = await _client.GetAsync("/api/v1/purchases/delivery-methods");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetDeliveryMethods_Authenticated_AlsoReturnsOk()
    {
        var token = await RegisterAndLogin("delivery_user@example.com", "Password123!");
        SetAuthHeader(token);

        var response = await _client.GetAsync("/api/v1/purchases/delivery-methods");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
