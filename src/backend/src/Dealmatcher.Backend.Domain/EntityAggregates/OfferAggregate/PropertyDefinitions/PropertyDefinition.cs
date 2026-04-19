namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.PropertyDefinitions;

public abstract class PropertyDefinition : DealmatcherEntityBase
{
    public string Name { get; init; } = null!;
    public PropertyType Type { get; init; }

    public PropertyDefinition(string name, PropertyType type)
    {
        Name = name;
        Type = type;
    }

    protected PropertyDefinition() { }

    public abstract Property CreatePropertyFromString(string value);
}

public abstract class PropertyDefinition<T> : PropertyDefinition where T : IParsable<T>
{
    public PropertyDefinition(string name, PropertyType type) : base(name, type) { }

    protected PropertyDefinition() { }

    public abstract Property<T> CreatePropertyTyped(T value);
    public override Property CreatePropertyFromString(string value)
    {
        return CreatePropertyTyped(T.Parse(value, null));
    }
}
