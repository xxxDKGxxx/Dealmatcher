namespace Dealmatcher.Backend.Domain.Core.Filtering;

public sealed class NumericPropertyFilter(
    PropertyDefinition propertyDefinition,
    double minValue,
    double maxValue) : PropertyFilter(propertyDefinition)
{
    public double MinValue { get; init; } = minValue;
    public double MaxValue { get; init; } = maxValue;

    public override void ApplyFilter(ISpecificationBuilder<Offer> query)
    {
        query.Where(o => o.Properties.OfType<NumericProperty>()
                            .Any(p => p.PropertyDefinition.Id == PropertyDefinition.Id
                                        && p.Value >= MinValue && p.Value <= MaxValue));
    }
}
