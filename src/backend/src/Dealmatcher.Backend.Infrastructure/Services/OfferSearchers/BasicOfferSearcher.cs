namespace Dealmatcher.Backend.Infrastructure.Services.OfferSearchers;

internal class BasicOfferSearcher : IOfferSearcher
{
    public async Task<IEnumerable<Offer>> SearchOffers(IReadRepository<Offer> offerRepository, int categoryId, decimal minPrice, decimal maxPrice, IEnumerable<string> tags, string searchPhrase, int limit, CancellationToken cancellationToken)
    {
        var basicOfferSearchSpecification = new OfferSearchSpecificationBase(categoryId, minPrice, maxPrice, tags, searchPhrase, limit);

        return await offerRepository.ListAsync(basicOfferSearchSpecification, cancellationToken);
    }
}
