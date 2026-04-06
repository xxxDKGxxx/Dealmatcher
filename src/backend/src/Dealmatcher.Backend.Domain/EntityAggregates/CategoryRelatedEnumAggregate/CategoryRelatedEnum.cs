namespace Dealmatcher.Backend.Domain.EntityAggregates.CategoryRelatedEnumAggregate;

public sealed class CategoryRelatedEnum : DealmatcherEntityBase
{
    public string Name { get; init; } = null!;

    private readonly List<CategoryRelatedEnumValue> _values = [];
    public IReadOnlyCollection<CategoryRelatedEnumValue> Values => _values.AsReadOnly();

    public CategoryRelatedEnum(string name)
    {
        Name = name;
    }

    private CategoryRelatedEnum() { }

    public override void Delete()
    {
        foreach (var item in _values)
        {
            item.Delete();
        }
    }
}
