namespace Dealmatcher.Backend.Domain.EntityAggregates.ActivityAggregate.Specifications;

public sealed class ActivitiesByUserIdFromToSpec : Specification<Activity>
{
    public ActivitiesByUserIdFromToSpec(int userId, DateTime from, DateTime to)
    {
        Query.Where(a => a.User.Id == userId && a.CreatedAt >= from && a.CreatedAt <= to);
    }
}
