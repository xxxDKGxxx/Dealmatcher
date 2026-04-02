namespace Dealmatcher.Backend.Domain.EntityAggregates.UserAggregate.Specifications;

public sealed class ActiveOrBannedUserByEmailSpec : SingleResultSpecification<User>
{
    private static readonly int _activeValue = UserStatus.Active.Value;
    private static readonly int _bannedValue = UserStatus.Banned.Value;
    public ActiveOrBannedUserByEmailSpec(string email)
    {
        Query.Where(u => u.Email == email)
             .Where(u => (int)(object)u.Status == _activeValue ||
                         (int)(object)u.Status == _bannedValue);
    }
}
