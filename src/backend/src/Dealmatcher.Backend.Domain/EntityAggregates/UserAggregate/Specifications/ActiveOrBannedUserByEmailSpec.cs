namespace Dealmatcher.Backend.Domain.EntityAggregates.UserAggregate.Specifications;

public sealed class ActiveOrBannedUserByEmailSpec : SingleResultSpecification<User>
{
    public ActiveOrBannedUserByEmailSpec(string email)
    {
        Query.Where(u => u.Email == email &&
                        (u.Status == UserStatus.Active || u.Status == UserStatus.Banned));
    }
}
