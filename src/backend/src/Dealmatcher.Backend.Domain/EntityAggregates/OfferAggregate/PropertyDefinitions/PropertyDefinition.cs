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

    public abstract Property CreatePropertyString(string value);
    public abstract Property CreateProperty(object value);
}

public abstract class PropertyDefinition<T> : PropertyDefinition where T : IParsable<T>
{
    public PropertyDefinition(string name, PropertyType type) : base(name, type) { }

    protected PropertyDefinition() { }

    public abstract Property<T> CreatePropertyTyped(T value);
    public override Property CreatePropertyString(string value)
    {
        return CreatePropertyTyped(T.Parse(value, null));
    }
    public override Property CreateProperty(object value)
    {
        return CreatePropertyTyped((T)Convert.ChangeType(value, typeof(T))!);
    }
}
