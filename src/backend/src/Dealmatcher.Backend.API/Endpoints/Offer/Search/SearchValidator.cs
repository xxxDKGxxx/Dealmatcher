namespace Dealmatcher.Backend.API.Endpoints.Offer.Search;

public sealed class SearchValidator : Validator<SearchRequest>
{
    public SearchValidator()
    {
        RuleFor(x => x.Tags).NotNull();
        RuleFor(x => x.Properties).NotNull();
        RuleFor(x => x.MinPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MaxPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MaxPrice).GreaterThanOrEqualTo(x => x.MinPrice);
        RuleFor(x => x.Limit).GreaterThan(0);
    }
}
