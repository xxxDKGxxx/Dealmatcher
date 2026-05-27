namespace Dealmatcher.Backend.Domain.EntityAggregates.PurchaseAggregate;

public sealed class Purchase : DealmatcherEntityBase, IAggregateRoot
{
    public User Buyer { get; private set; } = null!;
    public Offer Offer { get; private set; } = null!;
    public int Quantity { get; private set; }
    public decimal TotalPrice { get; private set; }
    public string DeliveryProviderId { get; private set; } = null!;
    public string PaymentProviderId { get; private set; } = null!;
    public string? PaymentSessionId { get; private set; }
    public PurchaseStatus Status { get; private set; } = PurchaseStatus.Pending;

    public Purchase(
        User buyer,
        Offer offer,
        int quantity,
        decimal totalPrice,
        string deliveryProviderId,
        string paymentProviderId)
    {
        Buyer = buyer;
        Offer = offer;
        Quantity = quantity;
        TotalPrice = totalPrice;
        DeliveryProviderId = deliveryProviderId;
        PaymentProviderId = paymentProviderId;
    }

    private Purchase() { }

    public void SetPaymentSession(PaymentSession session)
    {
        PaymentSessionId = session.SessionId;
    }

    public void Complete()
    {
        if (Status.IsFinished)
            throw new InvalidOperationException($"Cannot complete purchase with status {Status}");
        Status = PurchaseStatus.Completed;
    }

    public void Fail()
    {
        if (Status.IsFinished)
            throw new InvalidOperationException($"Cannot fail purchase with status {Status}");
        Status = PurchaseStatus.Failed;
    }
}
