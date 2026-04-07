namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.Properties;

public abstract class Property : DealmatcherEntityBase
{
    public PropertyDefinition PropertyDefinition { get; private set; } = null!;
    public Offer Offer { get; private set; } = null!;

    public Property(PropertyDefinition propertyDefinition, Offer offer)
    {
        PropertyDefinition = propertyDefinition;
        Offer = offer;
    }

    protected Property() { }
}

