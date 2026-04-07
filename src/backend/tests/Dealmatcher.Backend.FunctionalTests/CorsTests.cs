namespace Dealmatcher.Backend.FunctionalTests;

public class CorsTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient(new WebApplicationFactoryClientOptions
    {
        AllowAutoRedirect = false
    });

    [Fact]
    public async Task OptionsRequest_ReturnsCorrectCorsHeaders()
    {
        // Arrange
        var origin = "http://localhost:8080";
        var request = new HttpRequestMessage(HttpMethod.Options, "/api/v1/users/login");
        request.Headers.Add("Origin", origin);
        request.Headers.Add("Access-Control-Request-Method", "POST");
        request.Headers.Add("Access-Control-Request-Headers", "Content-Type");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var allowOrigin = response.Headers.GetValues("Access-Control-Allow-Origin").FirstOrDefault();
        allowOrigin.ShouldBe(origin);

        var allowMethods = response.Headers.GetValues("Access-Control-Allow-Methods").FirstOrDefault();
        allowMethods?.ShouldContain("POST");

        var allowCredentials = response.Headers.GetValues("Access-Control-Allow-Credentials").FirstOrDefault();
        allowCredentials.ShouldBe("true");
    }

    [Fact]
    public async Task GetRequest_WithOrigin_ReturnsCorrectCorsHeaders()
    {
        // Arrange
        var origin = "http://localhost:8080";
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/users/login");
        request.Headers.Add("Origin", origin);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        // We check CORS headers regardless of the actual endpoint result
        if (response.Headers.TryGetValues("Access-Control-Allow-Origin", out var values))
        {
            values.ShouldContain(origin);
        }
        else
        {
            throw new Exception("Access-Control-Allow-Origin header is missing");
        }

        response.Headers.GetValues("Access-Control-Allow-Credentials").ShouldContain("true");
    }
}
