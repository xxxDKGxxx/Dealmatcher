namespace Dealmatcher.Backend.Domain.EntityAggregates.UserAggregate.Specifications;

public sealed class ActiveOrBannedUserByEmailSpec : SingleResultSpecification<User>
{
    public ActiveOrBannedUserByEmailSpec(string email)
    {
        var blockingStatuses = new[] { UserStatus.Active, UserStatus.Banned };

        Query.Where(u => u.Email == email)
             .Where(u => blockingStatuses.Contains(u.Status));
    }
}
