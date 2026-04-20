namespace Dealmatcher.Backend.Domain.Interfaces.OfferSearching;

public sealed class OfferSearchSpecificationBase : Specification<Offer>
{
    public OfferSearchSpecificationBase(int categoryId, decimal minPrice, decimal maxPrice, IEnumerable<string> tags, string searchPhrase, int limit)
    {
        Query.Where(o => o.Price >= minPrice && o.Price <= maxPrice)
            .Where(o => o.Tags.Any(t => tags.Contains(t)) || tags.Count() == 0)
            .Where(o => o.Title.Normalize().Contains(searchPhrase.Normalize()))
            .Where(o => o.Category.Id == categoryId)
            .Take(limit);
    }
}
