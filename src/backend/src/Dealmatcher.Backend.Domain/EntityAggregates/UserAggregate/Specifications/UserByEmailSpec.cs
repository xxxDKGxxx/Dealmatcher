namespace Dealmatcher.Backend.Domain.EntityAggregates.UserAggregate.Specifications;
public sealed class UserByEmailSpec : SingleResultSpecification<User>
{
    public UserByEmailSpec(string Email)
    {
        Query.Where(u => u.Email == Email);
    }
}
