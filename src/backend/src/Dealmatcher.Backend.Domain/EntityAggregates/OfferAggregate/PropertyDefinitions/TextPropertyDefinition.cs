namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.PropertyDefinitions;

public sealed class TextPropertyDefinition : PropertyDefinition<string>
{
    public TextPropertyDefinition(string name, PropertyType type) : base(name, type)
    {
        if (type != PropertyType.Text)
        {
            throw new ArgumentException($"Invalid PropertyType: {type} for {nameof(TextPropertyDefinition)}");
        }
    }

    public override Property<string> CreatePropertyTyped(string value)
    {
        return new TextProperty(this, value);
    }
}
