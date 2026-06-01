namespace Dealmatcher.Backend.API.Endpoints.Bans.Delete;

public sealed record DeleteBanRequest
{
    public int BanId { get; init; }
}
