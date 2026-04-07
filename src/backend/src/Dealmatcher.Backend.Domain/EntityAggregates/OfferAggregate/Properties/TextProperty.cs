namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.Properties;

internal class TextProperty : Property<string>
{
    public TextProperty(TextPropertyDefinition propertyDefinition, string value) : base(propertyDefinition, value) { }

    private TextProperty() { }

    public override void Delete() { }
}
