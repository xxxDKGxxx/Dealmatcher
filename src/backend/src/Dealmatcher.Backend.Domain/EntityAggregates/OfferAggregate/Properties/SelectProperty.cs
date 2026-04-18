namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.Properties;

public sealed class SelectProperty : Property<string>
{
    public SelectProperty(SelectPropertyDefinition propertyDefinition, string value) : base(propertyDefinition, value) { }

    private SelectProperty() { }

    public override void SetValue(string value)
    {
        if (!(PropertyDefinition as SelectPropertyDefinition)?.Values.Contains(value) ?? throw new InvalidCastException($"{PropertyDefinition.Name} is not a {nameof(SelectPropertyDefinition)}"))
        {
            throw new ArgumentException($"Invalid {nameof(SelectProperty)} value: '{value}' for {nameof(SelectPropertyDefinition)}: '{PropertyDefinition.Name}'");
        }
        else
        {
            Value = value;
        }
    }
}
