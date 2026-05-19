namespace Dealmatcher.Backend.Domain.EntityAggregates.PaymentMethodAggregate;

public sealed class PaymentMethod : DealmatcherEntityBase, IAggregateRoot
{
    public string StringId { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string ProviderName { get; init; } = null!;
    public string Icon { get; private set; } = null!;

    private PaymentMethod() { }

    public PaymentMethod(string stringId, string name, string providerName, string icon)
    {
        StringId = stringId;
        Name = name;
        ProviderName = providerName;
        Icon = icon;
    }
}
