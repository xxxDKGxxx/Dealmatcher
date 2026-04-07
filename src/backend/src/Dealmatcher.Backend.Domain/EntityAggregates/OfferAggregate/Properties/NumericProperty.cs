namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.Properties;

public sealed class NumericProperty : Property<double>
{
    public NumericProperty(PropertyDefinition<double> propertyDefinition, double value) : base(propertyDefinition, value) { }

    private NumericProperty() { }

    public override void Delete() { }
}
