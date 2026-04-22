namespace Dealmatcher.Backend.Infrastructure.Services.OfferSuggestionServices;

public sealed class RandomOfferSuggestionService : IOfferSuggestionService
{
    public Task<IEnumerable<Offer>> SuggestOffers(IEnumerable<Offer> offers, int limit, CancellationToken cancellationToken)
    {
        return Task.FromResult(offers.OrderBy(_ => new Guid()).Take(limit));
    }
}
