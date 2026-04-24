namespace Dealmatcher.Backend.Domain.Core.Filtering;

public sealed class TagFilter(List<string> tags) : IFilter
{
    private readonly List<string> _tags = tags;
    public IReadOnlyCollection<string> Tags => _tags.AsReadOnly();

    public void ApplyFilter(ISpecificationBuilder<Offer> query)
    {
        if (Tags.Count != 0)
        {
            query.Where(o => o.Tags.Any(t => _tags.Contains(t)));
        }
    }
}
