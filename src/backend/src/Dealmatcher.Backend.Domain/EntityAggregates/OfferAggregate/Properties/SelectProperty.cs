namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.Properties;

public sealed class SelectProperty : Property<int>
{
    public SelectProperty(SelectPropertyDefinition propertyDefinition, int value) : base(propertyDefinition, value) { }

    private SelectProperty() { }

    public override void Delete() { }
}
