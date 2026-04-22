namespace Dealmatcher.Backend.Domain.Interfaces.OfferSuggestion;

public interface IOfferSuggestionService
{
    Task<IEnumerable<Offer>> SuggestOffers(IEnumerable<Offer> offers, int limit, CancellationToken cancellationToken);
}
