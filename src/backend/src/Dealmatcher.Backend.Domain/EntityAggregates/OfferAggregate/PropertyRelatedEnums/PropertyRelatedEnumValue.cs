namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.PropertyRelatedEnums;

public sealed class PropertyRelatedEnumValue : DealmatcherEntityBase
{
    public PropertyRelatedEnum PropertyRelatedEnum { get; private set; } = null!;
    public int Value { get; init; }
    public string Name { get; init; } = null!;

    public PropertyRelatedEnumValue(PropertyRelatedEnum propertyRelatedEnum, int value, string name)
    {
        PropertyRelatedEnum = propertyRelatedEnum;
        Value = value;
        Name = name;
    }

    private PropertyRelatedEnumValue() { }

    public override void Delete() { }
}
