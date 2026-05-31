namespace Dealmatcher.Backend.Domain.EntityAggregates.UserAggregate.Specifications;

public sealed class UsersWithExpiredBansSpec : Specification<User>
{
    public UsersWithExpiredBansSpec()
    {
        Query.Where(u => u.Status == UserStatus.Banned &&
                         u.Bans.Any(b => b.IsActive && b.ExpiresAt != null && b.ExpiresAt < DateTime.UtcNow))
             .Include(u => u.Bans);
    }
}
