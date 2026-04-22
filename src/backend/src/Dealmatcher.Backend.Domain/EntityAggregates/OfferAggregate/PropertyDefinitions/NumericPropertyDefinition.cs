using Dealmatcher.Backend.Domain.Core.Filtering;

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

    public override PropertyFilter CreatePropertyFilterTyped(List<double> values)
    {
        if (values.Count != 2)
        {
            throw new ArgumentException($"Invalid value count for NumericPropertyFilter: {values.Count}, should be 2");
        }

        return new NumericPropertyFilter(this, values[0], values[1]);
    }
}
