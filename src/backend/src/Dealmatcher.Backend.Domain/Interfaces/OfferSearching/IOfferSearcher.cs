namespace Dealmatcher.Backend.Domain.Interfaces.OfferSearching;

public interface IOfferSearcher
{
    Task<IEnumerable<Offer>> SearchOffers(IReadRepository<Offer> offerRepository, int categoryId, decimal minPrice, decimal maxPrice, IEnumerable<string> tags, string searchPhrase, int limit, CancellationToken cancellationToken);  // TODO: IEnumerable<Filter>
}
