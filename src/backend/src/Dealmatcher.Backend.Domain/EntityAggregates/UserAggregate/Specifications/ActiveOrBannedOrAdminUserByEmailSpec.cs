namespace Dealmatcher.Backend.Domain.EntityAggregates.UserAggregate.Specifications;

public sealed class ActiveOrBannedOrAdminUserByEmailSpec : SingleResultSpecification<User>
{
    public ActiveOrBannedOrAdminUserByEmailSpec(string email)
    {
        var blockingStatuses = new[] { UserStatus.Active, UserStatus.Banned, UserStatus.Admin };

        Query.Where(u => u.Email == email)
             .Where(u => blockingStatuses.Contains(u.Status));
    }
}
