namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.PropertyDefinitions;

public sealed class PropertyDefinition : DealmatcherEntityBase
{
    public string Name { get; init; } = null!;
    public PropertyType Type { get; init; }
    public PropertyRelatedEnum? PropertyRelatedEnum { get; private set; }
    public Category Category { get; private set; } = null!;

    public PropertyDefinition(string name, PropertyType type, Category category, PropertyRelatedEnum? propertyRelatedEnum = null)
    {
        if (type == PropertyType.Select && propertyRelatedEnum is null)
        {
            throw new ArgumentException("SelectProperty requires PropertyRelatedEnumId");
        }

        Name = name;
        Type = type;
        Category = category;
        PropertyRelatedEnum = propertyRelatedEnum;
    }

    private PropertyDefinition() { }

    public Property CreateProperty(Offer offer, object value)
    {
        return Type switch
        {
            PropertyType.Boolean => new BooleanProperty(this, offer, Convert.ToBoolean(value)),
            PropertyType.Numeric => new NumericProperty(this, offer, Convert.ToDouble(value)),
            PropertyType.Select => new SelectProperty(this, offer, Convert.ToInt32(value)),
            PropertyType.Text => new TextProperty(this, offer, Convert.ToString(value)!),
            _ => throw new ArgumentException("Invalid property type")
        };
    }

    public override void Delete() { }
}
