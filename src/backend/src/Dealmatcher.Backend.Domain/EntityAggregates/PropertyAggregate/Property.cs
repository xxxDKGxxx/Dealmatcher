namespace Dealmatcher.Backend.Domain.EntityAggregates.PropertyAggregate;

public abstract class Property : DealmatcherEntityBase
{
    public int PropertyDefinitionId { get; init; }
    public PropertyDefinition PropertyDefinition { get; private set; } = null!;
    public int OfferId { get; init; }
    public Offer Offer { get; private set; } = null!;

    public Property(int propertyDefinitionId, int offerId)
    {
        PropertyDefinitionId = propertyDefinitionId;
        OfferId = offerId;
    }

    protected Property() { }
}

