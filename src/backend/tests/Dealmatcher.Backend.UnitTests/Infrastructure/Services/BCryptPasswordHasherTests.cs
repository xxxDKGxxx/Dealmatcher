namespace Dealmatcher.Backend.UnitTests.Infrastructure.Services;

public class BCryptPasswordHasherTests
{
    private readonly BCryptPasswordHasher _hasher = new();

    [Fact]
    public void HashPassword_ReturnsDifferentHashEachTime()
    {
        var hash1 = _hasher.HashPassword("password123");
        var hash2 = _hasher.HashPassword("password123");

        hash1.ShouldNotBe(hash2);
    }

    [Fact]
    public void VerifyPassword_CorrectPassword_ReturnsTrue()
    {
        var hash = _hasher.HashPassword("password123");

        _hasher.VerifyPassword("password123", hash).ShouldBeTrue();
    }

    [Fact]
    public void VerifyPassword_WrongPassword_ReturnsFalse()
    {
        var hash = _hasher.HashPassword("password123");

        _hasher.VerifyPassword("wrongpassword", hash).ShouldBeFalse();
    }

    [Fact]
    public void HashPassword_ReturnsNonEmptyString()
    {
        var hash = _hasher.HashPassword("password123");

        hash.ShouldNotBeNullOrWhiteSpace();
    }
}
