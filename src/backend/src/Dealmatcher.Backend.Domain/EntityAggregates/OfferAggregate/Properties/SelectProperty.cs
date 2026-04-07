namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.Properties;

internal class SelectProperty : Property
{
    public int Value { get; private set; }

    public SelectProperty(PropertyDefinition propertyDefinition, Offer offer, int value) : base(propertyDefinition, offer)
    {
        Value = value;
    }

    private SelectProperty() { }

    public void SetValue(int value)
    {
        Value = value;
    }

    public override void Delete() { }
}
