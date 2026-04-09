namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.PropertyRelatedEnums;

public sealed class PropertyRelatedEnum : DealmatcherEntityBase
{
    public string Name { get; init; } = null!;

    private readonly List<PropertyRelatedEnumValue> _values = [];
    public IReadOnlyCollection<PropertyRelatedEnumValue> Values => _values.AsReadOnly();

    public PropertyRelatedEnum(string name)
    {
        Name = name;
    }

    private PropertyRelatedEnum() { }

    public override void Delete()
    {
        foreach (var item in _values)
        {
            item.Delete();
        }
    }
}
