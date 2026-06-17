namespace Dealmatcher.Backend.UseCases.Features.Bans.Get;

public sealed class GetBansQueryHandler(
    IReadRepository<User> userRepository,
    IMapper mapper) : IQueryHandler<GetBansQuery, Result<List<BanDto>>>
{
    public async Task<Result<List<BanDto>>> Handle(GetBansQuery request, CancellationToken cancellationToken)
    {
        var admin = await userRepository.GetByIdAsync(request.AdminId, cancellationToken);
        if (admin is null)
        {
            return Result.Unauthorized();
        }

        if (!admin.IsPrivileged)
        {
            return Result.Forbidden();
        }

        var spec = new UsersWithFilteredBansSpec(request.UserId, request.Active);
        var usersWithBans = await userRepository.ListAsync(spec, cancellationToken);

        var allBans = usersWithBans.SelectMany(u => u.Bans);

        var sortedBans = allBans.OrderByDescending(b => b.IssuedAt).ToList();

        return Result.Success(sortedBans.Select(mapper.Map<BanDto>).ToList());
    }
}
