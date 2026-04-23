namespace Dealmatcher.Backend.Domain.Core.Filtering;

public sealed class SearchPhraseFilter(string searchPhrase) : IFilter
{
    public string SearchPhrase { get; init; } = searchPhrase;

    public void ApplyFilter(ISpecificationBuilder<Offer> query)
    {
        if (!string.IsNullOrWhiteSpace(SearchPhrase))
        {
            query.Where(o => o.Title.StartsWith(SearchPhrase) || o.Title.EndsWith(SearchPhrase)
                                || o.Description.StartsWith(SearchPhrase) || o.Description.EndsWith(SearchPhrase));
        }
    }
}
