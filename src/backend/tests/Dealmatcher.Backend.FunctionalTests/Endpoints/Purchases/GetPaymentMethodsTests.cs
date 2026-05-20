namespace Dealmatcher.Backend.FunctionalTests.Endpoints.Purchases;

public class GetPaymentMethodsTests(CustomWebApplicationFactory factory) : EndpointTestBase(factory)
{
    [Fact]
    public async Task GetPaymentMethods_ReturnsOkWithPaymentMethods()
    {
        ClearAuthHeader();

        var response = await _client.GetAsync("/api/v1/purchases/payment-methods");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        json.RootElement.GetArrayLength().ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task GetPaymentMethods_ReturnsCorrectStructure()
    {
        ClearAuthHeader();

        var response = await _client.GetAsync("/api/v1/purchases/payment-methods");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        var firstMethod = json.RootElement[0];

        firstMethod.TryGetProperty("id", out _).ShouldBeTrue();
        firstMethod.TryGetProperty("name", out _).ShouldBeTrue();
        firstMethod.TryGetProperty("provider", out _).ShouldBeTrue();
        firstMethod.TryGetProperty("icon", out _).ShouldBeTrue();
    }

    [Fact]
    public async Task GetPaymentMethods_IsAnonymous_ReturnsOk()
    {
        ClearAuthHeader();

        var response = await _client.GetAsync("/api/v1/purchases/payment-methods");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPaymentMethods_Authenticated_AlsoReturnsOk()
    {
        var token = await RegisterAndLogin("payment_user@example.com", "Password123!");
        SetAuthHeader(token);

        var response = await _client.GetAsync("/api/v1/purchases/payment-methods");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
