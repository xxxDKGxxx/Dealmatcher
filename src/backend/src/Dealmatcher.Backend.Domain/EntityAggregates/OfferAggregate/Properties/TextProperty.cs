namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.Properties;

internal class TextProperty : Property
{
    public string Value { get; private set; } = null!;

    public TextProperty(PropertyDefinition propertyDefinition, Offer offer, string value) : base(propertyDefinition, offer)
    {
        Value = value;
    }

    private TextProperty() { }

    public void SetValue(string value)
    {
        Value = value;
    }

    public override void Delete() { }
}
