namespace Dealmatcher.Backend.Domain.Interfaces.Filtering;

public abstract class PropertyFilter(PropertyDefinition propertyDefinition) : IFilter
{
    public PropertyDefinition PropertyDefinition { get; init; } = propertyDefinition;

    public abstract void ApplyFilter(ISpecificationBuilder<Offer> query);
}
