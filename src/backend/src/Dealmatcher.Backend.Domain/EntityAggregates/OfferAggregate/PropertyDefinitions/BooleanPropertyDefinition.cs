using Dealmatcher.Backend.Domain.Core.Filtering;

namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.PropertyDefinitions;

public sealed class BooleanPropertyDefinition : PropertyDefinition<bool>
{
    public BooleanPropertyDefinition(string name, PropertyType type) : base(name, type)
    {
        if (type != PropertyType.Boolean)
        {
            throw new ArgumentException($"Invalid PropertyType: {type} for {nameof(BooleanPropertyDefinition)}");
        }
    }

    public override Property<bool> CreatePropertyTyped(bool value)
    {
        return new BooleanProperty(this, value);
    }

    public override PropertyFilter CreatePropertyFilterTyped(List<bool> values)
    {
        if (values.Count != 1)
        {
            throw new ArgumentException($"Invalid value count for BooleanPropertyFilter: {values.Count}, should be 1");
        }

        return new BooleanPropertyFilter(this, values[0]);
    }
}
