namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.Categories;

public sealed class Category(string name, string description) : DealmatcherEntityBase, IAggregateRoot
{
    public string Name { get; private set; } = name;
    public string Description { get; private set; } = description;
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

    public void UpdateName(string name)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            Name = name;
        }
    }

    public void UpdateDescription(string description)
    {
        if (!string.IsNullOrWhiteSpace(description))
        {
            Description = description;
        }
    }

    public override void Delete() { }
}
