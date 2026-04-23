namespace Dealmatcher.Backend.Domain.Core.Filtering;

public sealed class SearchPhraseFilter(string searchPhrase) : IFilter
{
    public string SearchPhrase { get; init; } = searchPhrase;

    public void ApplyFilter(ISpecificationBuilder<Offer> query)
    {
        if (!string.IsNullOrWhiteSpace(SearchPhrase))
        {
            query.Where(o => o.Title.Contains(SearchPhrase) || o.Description.Contains(SearchPhrase));
        }
    }
}
