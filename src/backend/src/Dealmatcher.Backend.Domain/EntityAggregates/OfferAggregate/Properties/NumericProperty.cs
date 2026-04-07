namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.Properties;

public sealed class NumericProperty : Property
{
    public double Value { get; private set; }

    public NumericProperty(PropertyDefinition propertyDefinition, Offer offer, double value) : base(propertyDefinition, offer)
    {
        Value = value;
    }

    private NumericProperty() { }

    public void SetValue(double value)
    {
        Value = value;
    }

    public override void Delete() { }
}
