using Microsoft.IdentityModel.JsonWebTokens;

namespace Dealmatcher.Backend.API.Services;

public class ClaimsPrincipalManager : IClaimsPrincipalManager
{
    public int? GetUserId(ClaimsPrincipal claimsPrincipal)
    {
        if (!int.TryParse(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            return null;
        }

        return userId;
    }
}
