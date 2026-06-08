namespace Dealmatcher.Backend.Domain.EntityAggregates.ActivityAggregate.Specifications;

public sealed class ActivitiesByUserIdFromToSpec : Specification<Activity>
{
    public ActivitiesByUserIdFromToSpec(int userId, DateTime? from, DateTime? to)
    {
        Query.Where(a => a.User.Id == userId);

        if (from is not null)
        {
            Query.Where(a => a.CreatedAt >= from);
        }

        if (to is not null)
        {
            Query.Where(a => a.CreatedAt <= to);
        }
    }
}
