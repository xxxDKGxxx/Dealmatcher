using Dealmatcher.Backend.Domain.Interfaces.OfferSuggestion;

namespace Dealmatcher.Backend.Infrastructure.Services.OfferSuggestionServices;

internal class BasicOfferSuggestionService : IOfferSuggestionService
{
    public async Task<IEnumerable<Offer>> SuggestOffers(IReadRepository<Offer> offerRepository, OfferSearchParameters parameters, CancellationToken cancellationToken)
    {
        var basicOfferSearchSpecification = new OfferSearchSpecificationBase(parameters);

        return await offerRepository.ListAsync(basicOfferSearchSpecification, cancellationToken);
    }
}
