namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate;

public abstract class OfferStatus(string name, int value) : SmartEnum<OfferStatus>(name, value)
{
    public static readonly OfferStatus Active = new ActiveStatus();
    public static readonly OfferStatus Deleted = new DeletedStatus();
    public static readonly OfferStatus Sold = new SoldStatus();

    public abstract bool CanBeSold { get; }

    private sealed class ActiveStatus() : OfferStatus("ACTIVE", 2)
    {
        public override bool CanBeSold => true;
    }

    private sealed class DeletedStatus() : OfferStatus("DELETED", 4)
    {
        public override bool CanBeSold => false;
    }

    private sealed class SoldStatus() : OfferStatus("SOLD", 5)
    {
        public override bool CanBeSold => false;
    }
}
