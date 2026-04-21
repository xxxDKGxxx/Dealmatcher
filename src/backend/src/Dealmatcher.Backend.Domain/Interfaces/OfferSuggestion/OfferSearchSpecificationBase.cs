namespace Dealmatcher.Backend.Domain.Interfaces.OfferSuggestion;

// TODO: filtrowanie po podstawowych parametrach
public sealed class OfferSearchSpecificationBase : Specification<Offer>
{
    public OfferSearchSpecificationBase(OfferSearchParameters parameters)
    {
        Query.Take(parameters.Limit);
    }
}
