namespace Dealmatcher.Backend.Domain.EntityAggregates.UserAggregate.Specifications;

public sealed class PagedUsersByStatusSpec : Specification<User>
{
    public PagedUsersByStatusSpec(int page, int limit, UserStatus status)
    {
        Query.IgnoreQueryFilters();
        Query.Where(u => u.Status == status)
            .Skip((page - 1) * limit).Take(limit);
    }
}
