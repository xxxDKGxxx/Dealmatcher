namespace Dealmatcher.Backend.Domain.EntityAggregates.UserAggregate.Specifications;
public sealed class UserByIdSpec : SingleResultSpecification<User>
{
    public UserByIdSpec(int id)
    {
        Query.Where(u => u.Id == id);
    }
}
