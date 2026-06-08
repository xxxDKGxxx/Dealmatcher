namespace Dealmatcher.Backend.API.Endpoints.Bans.Get;

public sealed record GetBansRequest
{
    public int? UserId { get; init; }
    public bool? Active { get; init; }
}
