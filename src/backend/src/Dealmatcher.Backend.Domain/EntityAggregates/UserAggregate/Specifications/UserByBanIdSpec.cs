namespace Dealmatcher.Backend.Domain.EntityAggregates.UserAggregate.Specifications;

public sealed class UserByBanIdSpec : Specification<User>
{
    public UserByBanIdSpec(int banId)
    {
        Query.Where(u => u.Bans.Any(b => b.Id == banId))
             .Include(u => u.Bans);
    }
}
