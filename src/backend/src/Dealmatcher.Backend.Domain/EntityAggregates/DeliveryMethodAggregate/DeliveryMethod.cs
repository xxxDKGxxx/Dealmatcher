namespace Dealmatcher.Backend.Domain.EntityAggregates.DeliveryMethodAggregate;

public sealed class DeliveryMethod : DealmatcherEntityBase, IAggregateRoot
{
    public string StringId { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string Description { get; private set; } = null!;
    public decimal Price { get; private set; }
    public int EstimatedDays { get; private set; }

    private DeliveryMethod() { }

    public DeliveryMethod(string stringId, string name, string description, decimal price, int estimatedDays)
    {
        StringId = stringId;
        Name = name;
        Description = description;
        Price = price;
        EstimatedDays = estimatedDays;
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0) throw new ArgumentException("Price cannot be negative", nameof(newPrice));
        Price = newPrice;
    }

    public void UpdateEstimatedDays(int newEstimatedDays)
    {
        if (newEstimatedDays < 0) throw new ArgumentException("Estimated days cannot be negative", nameof(newEstimatedDays));
        EstimatedDays = newEstimatedDays;
    }

    public void UpdateDescription(string newDescription)
    {
        if (string.IsNullOrWhiteSpace(newDescription)) throw new ArgumentException("Description cannot be empty", nameof(newDescription));
        Description = newDescription;
    }
}
