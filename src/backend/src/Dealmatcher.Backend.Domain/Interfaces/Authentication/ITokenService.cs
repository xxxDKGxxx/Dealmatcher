namespace Dealmatcher.Backend.Domain.Interfaces.Authentication;

public interface ITokenService
{
    public string GenerateToken(User user);
    public Task<bool> ValidateTokenAsync(string token);
}
