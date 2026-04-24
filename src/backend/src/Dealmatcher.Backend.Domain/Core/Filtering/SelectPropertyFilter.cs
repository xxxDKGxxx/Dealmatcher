namespace Dealmatcher.Backend.Domain.Core.Filtering;

public sealed class SelectPropertyFilter(
    PropertyDefinition propertyDefinition,
    IEnumerable<string> values) : PropertyFilter(propertyDefinition)
{
    private readonly List<string> _values = [.. values];
    public IReadOnlyCollection<string> Values => _values.AsReadOnly();

    public override void ApplyFilter(ISpecificationBuilder<Offer> query)
    {
        query.Where(o => o.Properties.OfType<SelectProperty>()
                            .Any(p => p.PropertyDefinition.Id == PropertyDefinition.Id
                                        && _values.Contains(p.Value)));
    }
}
