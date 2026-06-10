namespace Dealmatcher.Backend.Domain.Core.Filtering;

public sealed class StatusFilter(OfferStatus status) : IFilter
{
    public void ApplyFilter(ISpecificationBuilder<Offer> query)
    {
        query.Where(o => o.Status == status);
    }
}
