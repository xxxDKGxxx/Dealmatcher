namespace Dealmatcher.Backend.Domain.Core.Filtering;

public sealed class FilteredOffersSpecification : Specification<Offer>
{
    public FilteredOffersSpecification(List<IFilter> filters)
    {
        foreach (var filter in filters)
        {
            filter.ApplyFilter(Query);
        }
    }
}
