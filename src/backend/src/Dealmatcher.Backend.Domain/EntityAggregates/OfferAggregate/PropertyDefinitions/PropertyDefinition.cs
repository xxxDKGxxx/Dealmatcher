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
    public abstract void UpdateProperty(Property property, string value);
    public abstract PropertyFilter CreatePropertyFilterFromStrings(List<string> values);
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

    public override void UpdateProperty(Property property, string value)
    {
        if (property is not Property<T> typedProperty)
        {
            throw new ArgumentException($"Invalid property type for definition {Name}. Expected Property<{typeof(T).Name}>, got {property.GetType().Name}");
        }
        typedProperty.SetValue(T.Parse(value, null));
    }

    public abstract PropertyFilter CreatePropertyFilterTyped(List<T> values);
    public override PropertyFilter CreatePropertyFilterFromStrings(List<string> values)
    {
        return CreatePropertyFilterTyped([.. values.Select(v => T.Parse(v, null))]);
    }
}
