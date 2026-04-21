using Dealmatcher.Backend.Domain.Interfaces.OfferSuggestion;

namespace Dealmatcher.Backend.UseCases.Features.Offers.Search;

public sealed class SearchOffersQueryHandler(
    IReadRepository<Offer> offerRepository,
    IReadRepository<Category> categoryRepository,
    IOfferSuggestionService offerSearcher,
    IMapper mapper) : IQueryHandler<SearchOffersQuery, Result<List<OfferDto>>>
{
    public async Task<Result<List<OfferDto>>> Handle(SearchOffersQuery request, CancellationToken cancellationToken)
    {
        if (request.CategoryId != null)
        {
            var categoryWithDefinitionsByIdSpec = new CategoryWithDefinitionsByIdSpec(request.CategoryId.Value!);
            var category = await categoryRepository.SingleOrDefaultAsync(categoryWithDefinitionsByIdSpec, cancellationToken);

            if (category == null)
            {
                return Result.Invalid(new ValidationError($"Category with id: {request.CategoryId} doesn't exist"));
            }
            // TODO: weryfikacja poprawności filtrów
        }

        if (request.MinPrice > request.MaxPrice)
        {
            return Result.Invalid(new ValidationError($"MinPrice ({request.MinPrice}) must be less than or equal to MaxPrice ({request.MaxPrice})"));
        }

        var offerSearchParameters = new OfferSearchParameters(
            request.CategoryId,
            request.MinPrice,
            request.MaxPrice,
            request.Tags,
            request.SearchPhrase,
            request.limit);

        var searchResults = await offerSearcher.SuggestOffers(offerRepository, offerSearchParameters, cancellationToken);
        if (!searchResults.Any())
        {
            return Result.NoContent();
        }

        return Result.Success(mapper.Map<List<OfferDto>>(searchResults));
    }
}
