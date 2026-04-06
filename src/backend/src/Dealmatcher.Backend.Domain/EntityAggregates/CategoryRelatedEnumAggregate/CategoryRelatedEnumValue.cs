namespace Dealmatcher.Backend.Domain.EntityAggregates.CategoryRelatedEnumAggregate;

public sealed class CategoryRelatedEnumValue : DealmatcherEntityBase
{
    public int CategoryRelatedEnumId { get; init; }
    public CategoryRelatedEnum CategoryRelatedEnum { get; private set; } = null!;
    public int Value { get; init; }
    public string Name { get; init; } = null!;

    public CategoryRelatedEnumValue(int categoryRelatedEnumId, int value, string name)
    {
        CategoryRelatedEnumId = categoryRelatedEnumId;
        Value = value;
        Name = name;
    }

    private CategoryRelatedEnumValue() { }

    public override void Delete()
    {
    }
}
