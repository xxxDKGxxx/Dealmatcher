namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.Properties;

public sealed class BooleanProperty : Property<bool>
{
    public BooleanProperty(BooleanPropertyDefinition propertyDefinition, bool value) : base(propertyDefinition, value) { }

    private BooleanProperty() { }

    public override void Delete() { }
}
