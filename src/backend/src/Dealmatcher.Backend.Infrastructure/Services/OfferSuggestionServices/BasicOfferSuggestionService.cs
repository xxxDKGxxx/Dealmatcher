using Dealmatcher.Backend.Domain.Interfaces.Filtering;
using Dealmatcher.Backend.Domain.Interfaces.OfferSuggestion;

namespace Dealmatcher.Backend.Infrastructure.Services.OfferSuggestionServices;

internal class BasicOfferSuggestionService : IOfferSuggestionService
{
    public Task<IEnumerable<Offer>> SuggestOffers(IEnumerable<Offer> offers, int limit, CancellationToken cancellationToken)
    {
        return Task.FromResult(offers.OrderBy(_ => new Guid()).Take(limit));
    }
}
