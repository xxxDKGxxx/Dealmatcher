namespace Dealmatcher.Backend.Domain.EntityAggregates.CategoryAggregate;

public sealed class Category : DealmatcherEntityBase
{
    public string Name { get; init; } = null!;
    private readonly List<PropertyDefinition> _propertyDefinitions = new();
    public IReadOnlyCollection<PropertyDefinition> PropertyDefinitions => _propertyDefinitions.AsReadOnly();

    public Category(string name)
    {
        Name = name;
    }

    public void AddPropertyDefinition(PropertyDefinition propertyDefinitions)
    {
        _propertyDefinitions.Add(propertyDefinitions);
    }
    public void RemovePropertyDefinition(PropertyDefinition propertyDefinition)
    {
        _propertyDefinitions.Remove(propertyDefinition);
    }
    public override void Delete()
    {
    }
}
