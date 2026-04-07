namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.Properties;

internal class SelectProperty : Property<int>
{
    public SelectProperty(SelectPropertyDefinition propertyDefinition, int value) : base(propertyDefinition, value) { }

    private SelectProperty() { }

    public override void Delete() { }
}
