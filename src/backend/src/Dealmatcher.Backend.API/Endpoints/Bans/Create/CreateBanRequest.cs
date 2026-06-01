namespace Dealmatcher.Backend.API.Endpoints.Bans.Create;

public sealed record CreateBanRequest(
    int UserId,
    string Reason,
    DateTime? ExpiresAt
);
