namespace Dealmatcher.Backend.Domain.EntityAggregates.UserAggregate.Specifications;

public sealed class UserByEmailSpec : SingleResultSpecification<User>
{
    public UserByEmailSpec(string email)
    {
        Query.Where(u => u.Email == email);
    }
}
