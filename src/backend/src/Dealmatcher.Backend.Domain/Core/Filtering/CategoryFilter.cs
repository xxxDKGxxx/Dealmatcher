namespace Dealmatcher.Backend.Domain.Core.Filtering;

public sealed class CategoryFilter(int? categoryId) : IFilter
{
    public int? CategoryId { get; init; } = categoryId;

    public void ApplyFilter(ISpecificationBuilder<Offer> query)
    {
        if (CategoryId is not null)
        {
            query.Where(o => o.Category.Id == CategoryId);
        }
    }
}
