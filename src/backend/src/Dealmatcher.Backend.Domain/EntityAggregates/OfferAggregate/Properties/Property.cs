namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.Properties;

public abstract class Property : DealmatcherEntityBase
{
    public PropertyDefinition PropertyDefinition { get; private set; } = null!;

    public Property(PropertyDefinition propertyDefinition)
    {
        PropertyDefinition = propertyDefinition;
    }

    protected Property() { }
}

public abstract class Property<T> : Property
{
    public T Value { get; private set; } = default!;

    public Property(PropertyDefinition<T> propertyDefinition, T value) : base(propertyDefinition)
    {
        Value = value;
    }

    protected Property() { }

    public virtual void SetValue(T value)
    {
        Value = value;
    }
}
