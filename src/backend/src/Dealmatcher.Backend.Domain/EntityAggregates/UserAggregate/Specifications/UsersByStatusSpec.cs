namespace Dealmatcher.Backend.Domain.EntityAggregates.UserAggregate.Specifications;

public sealed class UsersByStatusSpec : Specification<User>
{
    public UsersByStatusSpec(UserStatus status)
    {
        Query.IgnoreQueryFilters();
        Query.Where(u => u.Status == status);
    }
}
