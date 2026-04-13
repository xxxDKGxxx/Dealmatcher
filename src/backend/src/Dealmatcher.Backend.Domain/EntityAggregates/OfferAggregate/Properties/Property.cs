namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.Properties;

public abstract class Property : DealmatcherEntityBase
{
    public PropertyDefinition PropertyDefinition { get; private set; } = null!;
    public abstract string StringValue { get; }

    public Property(PropertyDefinition propertyDefinition)
    {
        PropertyDefinition = propertyDefinition;
    }

    protected Property() { }
}

public abstract class Property<T> : Property where T : IParsable<T>
{
    public T Value { get; protected set; } = default!;
    public override string StringValue => Value.ToString()!;

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
