namespace Dealmatcher.Backend.UseCases.Features.Bans.Get;

public sealed record GetBansQuery(
    int AdminId,
    int? UserId,
    bool? Active
) : IQuery<Result<List<BanDto>>>;
