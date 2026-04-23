namespace Dealmatcher.Backend.Domain.Core.Filtering;

public sealed class BooleanPropertyFilter(
    PropertyDefinition propertyDefinition,
    bool value) : PropertyFilter(propertyDefinition)
{
    public bool Value { get; init; } = value;

    public override void ApplyFilter(ISpecificationBuilder<Offer> query)
    {
        query.Where(o => o.Properties.OfType<BooleanProperty>()
                            .Any(p => p.PropertyDefinition.Id == PropertyDefinition.Id
                                        && p.Value == Value));
    }
}
