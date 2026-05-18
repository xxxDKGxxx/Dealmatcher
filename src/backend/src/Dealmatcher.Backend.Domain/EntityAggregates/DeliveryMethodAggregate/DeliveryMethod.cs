namespace Dealmatcher.Backend.Domain.EntityAggregates.DeliveryMethodAggregate;

public sealed class DeliveryMethod : DealmatcherEntityBase, IAggregateRoot
{
    public string StringId { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string Description { get; private set; } = null!;
    public string ProviderName { get; init; } = null!;
    public decimal Price { get; private set; }

    /* EF Core */
    private DeliveryMethod() { }

    public DeliveryMethod(string stringId, string name, string providerName, string description, decimal price)
    {
        StringId = stringId;
        Name = name;
        ProviderName = providerName;
        Description = description;
        Price = price;
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0) throw new ArgumentException("Price cannot be negative", nameof(newPrice));
        Price = newPrice;
    }

    public void UpdateDescription(string newDescription)
    {
        if (string.IsNullOrWhiteSpace(newDescription)) throw new ArgumentException("Description cannot be empty", nameof(newDescription));
        Description = newDescription;
    }
}
