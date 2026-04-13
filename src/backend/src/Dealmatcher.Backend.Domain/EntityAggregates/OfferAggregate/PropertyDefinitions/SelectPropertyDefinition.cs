namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.PropertyDefinitions;

public sealed class SelectPropertyDefinition : PropertyDefinition<string>
{
    private readonly List<string> _values = [];
    public IReadOnlyCollection<string> Values { get => _values.AsReadOnly(); }

    public SelectPropertyDefinition(string name, PropertyType type, List<string> values) : base(name, type)
    {
        if (type != PropertyType.Select)
        {
            throw new ArgumentException($"Invalid PropertyType: {type} for {nameof(SelectPropertyDefinition)}");
        }
        _values = values;
    }

    private SelectPropertyDefinition() { }

    public override Property<string> CreatePropertyTyped(string value)
    {
        if (!Values.Contains(value))
        {
            throw new ArgumentException($"Invalid {nameof(SelectProperty)} value: '{value}' for {nameof(SelectPropertyDefinition)}: '{Name}'");
        }
        return new SelectProperty(this, value);
    }
}
