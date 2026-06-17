namespace Dealmatcher.Backend.Domain.EntityAggregates.UserAggregate.Specifications;

public sealed class UsersWithFilteredBansSpec : Specification<User>
{
    public UsersWithFilteredBansSpec(int? userId, bool? isActive)
    {
        if (userId.HasValue)
        {
            Query.Where(u => u.Id == userId.Value);
        }

        if (isActive.HasValue)
        {
            Query.Include(u => u.Bans.Where(b => b.IsActive == isActive.Value))
                 .ThenInclude(b => b.IssuedBy);

            Query.Where(u => u.Bans.Any(b => b.IsActive == isActive.Value));
        }
        else
        {
            Query.Include(u => u.Bans)
                 .ThenInclude(b => b.IssuedBy);

            Query.Where(u => u.Bans.Any());
        }
    }
}
