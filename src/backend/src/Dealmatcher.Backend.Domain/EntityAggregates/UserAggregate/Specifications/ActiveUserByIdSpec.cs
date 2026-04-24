namespace Dealmatcher.Backend.Domain.EntityAggregates.UserAggregate.Specifications;

public sealed class ActiveUserByIdSpec : SingleResultSpecification<User>
{
    public ActiveUserByIdSpec(int id)
    {
        var allowedStatuses = new[] { UserStatus.Active };
        Query.Where(u => u.Id == id)
            .Where(u => allowedStatuses.Contains(u.Status));
    }
}
