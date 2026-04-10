namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.Properties;

public sealed class SelectProperty : Property<string>
{
    public SelectProperty(SelectPropertyDefinition propertyDefinition, string value) : base(propertyDefinition, value) { }

    private SelectProperty() { }

    public override void Delete() { }
}
