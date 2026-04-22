using Dealmatcher.Backend.Domain.Core.Filtering;
using Dealmatcher.Backend.Domain.Interfaces.Filtering;
using Dealmatcher.Backend.Domain.Interfaces.OfferSuggestion;

namespace Dealmatcher.Backend.UseCases.Features.Offers.Search;

public sealed class SearchOffersQueryHandler(
    IReadRepository<Offer> offerRepository,
    IReadRepository<Category> categoryRepository,
    IOfferSuggestionService offerSuggestionService,
    IMapper mapper) : IQueryHandler<SearchOffersQuery, Result<List<OfferDto>>>
{
    public async Task<Result<List<OfferDto>>> Handle(SearchOffersQuery request, CancellationToken cancellationToken)
    {
        List<IFilter> filters = [];
        if (request.CategoryId != null)
        {
            var categoryWithDefinitionsByIdSpec = new CategoryWithDefinitionsByIdSpec(request.CategoryId.Value!);
            var category = await categoryRepository.SingleOrDefaultAsync(categoryWithDefinitionsByIdSpec, cancellationToken);

            if (category == null)
            {
                return Result.Invalid(new ValidationError($"Category with id: {request.CategoryId} doesn't exist"));
            }

            foreach (var propertyId in request.PropertyFilters.Keys)
            {
                if (!int.TryParse(propertyId, out int propertyIdParsed))
                {
                    return Result.Invalid(new ValidationError($"Invalid property Id: {propertyId}"));
                }

                var propertyDefinition = category.PropertyDefinitions.Where(pd => pd.Id == propertyIdParsed).FirstOrDefault();
                if (propertyDefinition is null)
                {
                    return Result.Invalid(new ValidationError($"Invalid property Id: {propertyId}"));
                }

                try
                {
                    var propertyFilter = propertyDefinition.CreatePropertyFilterFromStrings(request.PropertyFilters[propertyId]);
                    filters.Add(propertyFilter);
                }
                catch
                {
                    return Result.Invalid(new ValidationError($"Invalid property value: {request.PropertyFilters[propertyId]}"));
                }
            }
        }

        if (request.MinPrice > request.MaxPrice)
        {
            return Result.Invalid(new ValidationError($"MinPrice ({request.MinPrice}) must be less than or equal to MaxPrice ({request.MaxPrice})"));
        }

        filters.Add(new CategoryFilter(request.CategoryId));
        filters.Add(new PriceFilter(request.MinPrice, request.MaxPrice));
        filters.Add(new TagFilter(request.Tags));
        filters.Add(new SearchPhraseFilter(request.SearchPhrase));

        var filteredOffersSpecification = new FilteredOffersSpecification(filters);
        var offersFiltered = await offerRepository.ListAsync(filteredOffersSpecification, cancellationToken);

        var searchResults = await offerSuggestionService.SuggestOffers(offersFiltered, request.Limit, cancellationToken);

        if (!searchResults.Any())
        {
            return Result.NoContent();
        }

        return Result.Success(mapper.Map<List<OfferDto>>(searchResults));
    }
}
