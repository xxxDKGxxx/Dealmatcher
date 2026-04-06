namespace Dealmatcher.Backend.Domain.EntityAggregates.CategoryAggregate;

public sealed class Category(string name) : DealmatcherEntityBase
{
    public string Name { get; init; } = name;
    private readonly List<PropertyDefinition> _propertyDefinitions = [];
    public IReadOnlyCollection<PropertyDefinition> PropertyDefinitions => _propertyDefinitions.AsReadOnly();

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
