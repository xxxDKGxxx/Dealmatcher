namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.PropertyDefinitions;

public sealed class NumericPropertyDefinition : PropertyDefinition<double>
{
    public NumericPropertyDefinition(string name, PropertyType type) : base(name, type)
    {
        if (type != PropertyType.Numeric)
        {
            throw new ArgumentException($"Invalid PropertyType: {type} for {nameof(NumericPropertyDefinition)}");
        }
    }

    public override Property<double> CreatePropertyTyped(double value)
    {
        return new NumericProperty(this, value);
    }
}
