
using Dealmatcher.Backend.Domain.Interfaces.OfferSearching;

namespace Dealmatcher.Backend.UseCases.Features.Offers.Search;

public sealed class SearchOffersQueryHandler(
    IReadRepository<Offer> offerRepository,
    IReadRepository<Category> categoryRepository,
    IOfferSearcher offerSearcher,
    IMapper mapper) : IQueryHandler<SearchOffersQuery, Result<List<OfferDto>>>
{
    public async Task<Result<List<OfferDto>>> Handle(SearchOffersQuery request, CancellationToken cancellationToken)
    {
        var categoryWithDefinitionsByIdSpec = new CategoryWithDefinitionsByIdSpec(request.CategoryId);
        var category = await categoryRepository.SingleOrDefaultAsync(categoryWithDefinitionsByIdSpec, cancellationToken);

        if (category == null) 
        {
            return Result.Invalid(new ValidationError($"Category with id: {request.CategoryId} doesn't exist"));
        }

        if (request.MinPrice > request.MaxPrice)
        {
            return Result.Invalid(new ValidationError($"MinPrice ({request.MinPrice}) must be less than or equal to MaxPrice ({request.MaxPrice})"));
        }

        var searchResults = await offerSearcher.SearchOffers(offerRepository, request.CategoryId, request.MinPrice, request.MaxPrice, request.Tags, request.SearchPhrase, request.limit, cancellationToken);
        if (searchResults.Count() == 0)
        {
            return Result.NoContent();
        }

        return Result.Success(mapper.Map<List<OfferDto>>(searchResults));
    }
}
