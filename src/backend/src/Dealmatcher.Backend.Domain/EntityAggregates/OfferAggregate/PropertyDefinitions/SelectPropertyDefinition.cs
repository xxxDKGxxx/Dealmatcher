namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.PropertyDefinitions;

public sealed class SelectPropertyDefinition : PropertyDefinition<int>
{
    public PropertyRelatedEnum PropertyRelatedEnum { get; private set; } = null!;

    public SelectPropertyDefinition(string name, PropertyType type, PropertyRelatedEnum propertyRelatedEnum) : base(name, type)
    {
        if (type != PropertyType.Select)
        {
            throw new ArgumentException($"Invalid PropertyType: {type} for {nameof(SelectPropertyDefinition)}");
        }
        PropertyRelatedEnum = propertyRelatedEnum;
    }

    private SelectPropertyDefinition() { }

    public override Property<int> CreatePropertyTyped(int value)
    {
        return new SelectProperty(this, value);
    }
}
