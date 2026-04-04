namespace Dealmatcher.Backend.API.Interfaces;

public interface IClaimsPrincipalManager
{
    int? GetUserId(ClaimsPrincipal claimsPrincipal);
}
