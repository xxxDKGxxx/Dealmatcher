namespace Dealmatcher.Backend.Domain.EntityAggregates.UserAggregate.Specifications;

public sealed class UsersWithExpiredBansSpec : Specification<User>
{
    public UsersWithExpiredBansSpec()
    {
        var currentTime = DateTime.UtcNow;
        Query
            .Where(u => u.Bans.Any(b => b.IsActive && b.ExpiresAt.HasValue && b.ExpiresAt.Value < currentTime))
             .Include(u => u.Bans);
    }
}
