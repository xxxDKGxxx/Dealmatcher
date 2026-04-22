namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.Categories.Specifications;

public class CategoryByNameSpec : SingleResultSpecification<Category>
{
    public CategoryByNameSpec(string categoryName)
    {
        Query.Where(c => c.Name == categoryName).Take(1);
    }
}
