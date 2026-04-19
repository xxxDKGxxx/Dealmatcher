namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.Properties;

public sealed class TextProperty : Property<string>
{
    public TextProperty(TextPropertyDefinition propertyDefinition, string value) : base(propertyDefinition, value) { }

    private TextProperty() { }
}
