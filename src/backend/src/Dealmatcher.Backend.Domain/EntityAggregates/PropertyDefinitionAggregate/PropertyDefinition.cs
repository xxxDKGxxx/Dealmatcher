namespace Dealmatcher.Backend.Domain.EntityAggregates.PropertyDefinitionAggregate;

public sealed class PropertyDefinition : DealmatcherEntityBase
{
    public string Name { get; init; } = null!;
    public PropertyType Type { get; init; }
    public int? CategoryRelatedEnumId { get; init; }
    public int CategoryId { get; init; }
    public Category Category { get; private set; } = null!;

    public PropertyDefinition(string name, PropertyType type, int categoryId, int? categoryRelatedEnumId = null)
    {
        if (type == PropertyType.Select && categoryRelatedEnumId is null)
        {
            throw new ArgumentException("SelectProperty requires CategoryRelatedEnumId");
        }
        
        Name = name;
        Type = type;
        CategoryId = categoryId;
        CategoryRelatedEnumId = categoryRelatedEnumId;
    }

    private PropertyDefinition() { }

    public Property CreateProperty(int offerId, object value)
    {
        return this.Type switch
        {
            PropertyType.Boolean => new BooleanProperty(Id, offerId, Convert.ToBoolean(value)),
            PropertyType.Numeric => new NumericProperty(Id, offerId, Convert.ToDouble(value)),
            PropertyType.Select => new SelectProperty(Id, offerId, Convert.ToInt32(value)),
            _ => throw new ArgumentException("Invalid property type")
        };
    }

    public override void Delete() { }
}
