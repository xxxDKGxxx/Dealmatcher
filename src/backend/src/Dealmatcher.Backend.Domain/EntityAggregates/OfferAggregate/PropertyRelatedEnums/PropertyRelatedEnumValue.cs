namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.PropertyRelatedEnums;

public sealed class PropertyRelatedEnumValue : DealmatcherEntityBase
{
    public PropertyRelatedEnum PropertyRelatedEnum { get; private set; } = null!;
    public string Value { get; init; } = null!;

    public PropertyRelatedEnumValue(PropertyRelatedEnum propertyRelatedEnum, string value)
    {
        PropertyRelatedEnum = propertyRelatedEnum;
        Value = value;
    }

    private PropertyRelatedEnumValue() { }

    public override void Delete() { }
}
