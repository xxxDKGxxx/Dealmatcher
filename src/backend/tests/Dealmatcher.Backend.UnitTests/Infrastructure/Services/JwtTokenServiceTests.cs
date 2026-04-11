namespace Dealmatcher.Backend.UnitTests.Infrastructure.Services;

public class JwtTokenServiceTests
{
    private readonly JwtTokenService _tokenService;

    private const string SecretKey = "this-is-a-test-secret-key-that-is-long-enough-32chars!";
    private const string Issuer = "test-issuer";
    private const string Audience = "test-audience";

    public JwtTokenServiceTests()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Authentication:Jwt:SecretKey"] = SecretKey,
                ["Authentication:Jwt:Issuer"] = Issuer,
                ["Authentication:Jwt:Audience"] = Audience,
            })
            .Build();

        var logger = Substitute.For<ILogger<JwtTokenService>>();
        _tokenService = new JwtTokenService(configuration, logger);
    }

    private static User CreateUser(bool isPrivileged = false)
    {
        var user = new User("test@example.com", "hashedpassword", "Jan", "Kowalski");
        if (isPrivileged)
        {
            user.GrantAdminPrivileges();
        }
        return user;
    }

    private static ClaimsPrincipal DecodeToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        return new ClaimsPrincipal(new ClaimsIdentity(jwt.Claims));
    }

    [Fact]
    public void GenerateToken_ReturnsNonEmptyString()
    {
        var user = CreateUser();

        var token = _tokenService.GenerateToken(user);

        token.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GenerateToken_ContainsCorrectEmail()
    {
        var user = CreateUser();

        var token = _tokenService.GenerateToken(user);
        var claims = DecodeToken(token);

        claims.FindFirst(JwtRegisteredClaimNames.Email)?.Value.ShouldBe("test@example.com");
    }

    [Fact]
    public void GenerateToken_ContainsCorrectSubject()
    {
        var user = CreateUser();

        var token = _tokenService.GenerateToken(user);
        var claims = DecodeToken(token);

        claims.FindFirst(JwtRegisteredClaimNames.Sub)?.Value.ShouldBe(user.Id.ToString());
    }

    [Fact]
    public void GenerateToken_RegularUser_HasUserRole()
    {
        var user = CreateUser(isPrivileged: false);

        var token = _tokenService.GenerateToken(user);
        var claims = DecodeToken(token);

        claims.FindFirst(ClaimTypes.Role)?.Value.ShouldBe("User");
    }

    [Fact]
    public void GenerateToken_PrivilegedUser_HasAdminRole()
    {
        var user = CreateUser(isPrivileged: true);

        var token = _tokenService.GenerateToken(user);
        var claims = DecodeToken(token);

        claims.FindFirst(ClaimTypes.Role)?.Value.ShouldBe("Admin");
    }

    [Fact]
    public void GenerateToken_HasCorrectIssuer()
    {
        var user = CreateUser();

        var token = _tokenService.GenerateToken(user);
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        jwt.Issuer.ShouldBe(Issuer);
    }

    [Fact]
    public void GenerateToken_HasCorrectAudience()
    {
        var user = CreateUser();

        var token = _tokenService.GenerateToken(user);
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        jwt.Audiences.ShouldContain(Audience);
    }

    [Fact]
    public async Task ValidateToken_ValidToken_ReturnsTrue()
    {
        var user = CreateUser();
        var token = _tokenService.GenerateToken(user);

        var result = await _tokenService.ValidateTokenAsync(token);

        result.ShouldBeTrue();
    }

    [Fact]
    public async Task ValidateToken_InvalidToken_ReturnsFalse()
    {
        var result = await _tokenService.ValidateTokenAsync("invalid.token.string");

        result.ShouldBeFalse();
    }

    [Fact]
    public async Task ValidateToken_TamperedToken_ReturnsFalse()
    {
        var user = CreateUser();
        var token = _tokenService.GenerateToken(user);
        var tampered = token + "x";

        var result = await _tokenService.ValidateTokenAsync(tampered);

        result.ShouldBeFalse();
    }
}
