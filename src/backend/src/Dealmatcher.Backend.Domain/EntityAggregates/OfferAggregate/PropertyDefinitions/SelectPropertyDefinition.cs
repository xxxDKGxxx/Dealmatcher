namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.PropertyDefinitions;

public sealed class SelectPropertyDefinition : PropertyDefinition<string>
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

    public override Property<string> CreatePropertyTyped(string value)
    {
        if (!PropertyRelatedEnum.Values.Any(pev => pev.Value == value))
        {
            throw new ArgumentException($"Invalid SelectProperty value:{value} for Enum:{PropertyRelatedEnum.Name}");
        }
        return new SelectProperty(this, value);
    }
}
