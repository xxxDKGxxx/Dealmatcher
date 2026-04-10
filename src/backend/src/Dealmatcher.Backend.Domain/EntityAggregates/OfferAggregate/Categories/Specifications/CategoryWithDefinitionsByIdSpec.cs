namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.Categories.Specifications;

public sealed class CategoryWithDefinitionsByIdSpec : SingleResultSpecification<Category>
{
    public CategoryWithDefinitionsByIdSpec(int id)
    {
        Query.Where(c => c.Id == id)
            .Include(c => c.PropertyDefinitions);
    }
}
