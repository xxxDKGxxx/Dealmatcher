namespace Dealmatcher.Backend.Domain.Interfaces.OfferSuggestion;

public interface IOfferSuggestionService
{
    Task<IEnumerable<Offer>> SuggestOffers(IReadRepository<Offer> offerRepository, OfferSearchParameters parameters, CancellationToken cancellationToken);
}
