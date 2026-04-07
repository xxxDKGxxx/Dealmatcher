namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.Properties;

internal class BooleanProperty : Property
{
    public bool Value { get; private set; }

    public BooleanProperty(PropertyDefinition propertyDefinition, Offer offer, bool value) : base(propertyDefinition, offer)
    {
        Value = value;
    }

    private BooleanProperty() { }

    public void SetValue(bool value)
    {
        Value = value;
    }

    public override void Delete() { }
}
