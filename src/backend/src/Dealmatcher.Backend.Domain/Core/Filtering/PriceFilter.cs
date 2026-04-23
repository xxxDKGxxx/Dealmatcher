namespace Dealmatcher.Backend.Domain.Core.Filtering;

public sealed class PriceFilter(decimal minPrice, decimal maxPrice) : IFilter
{
    public decimal MinPrice { get; init; } = minPrice;
    public decimal MaxPrice { get; init; } = maxPrice;

    public void ApplyFilter(ISpecificationBuilder<Offer> query)
    {
        query.Where(o => o.Price >= MinPrice && o.Price <= MaxPrice);
    }
}
