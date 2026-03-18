namespace Dealmatcher.Backend.Domain.EntityAggregates.ExampleAggregate.Specifications;

public sealed class ExampleByIdSpec : SingleResultSpecification<Example>
{
    public ExampleByIdSpec(int id)
    {
        Query.Where(e => e.Id == id);
    }
}
