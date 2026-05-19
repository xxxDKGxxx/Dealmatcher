namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate;

public abstract class OfferStatus(string name, int value) : SmartEnum<OfferStatus>(name, value)
{
    public static readonly OfferStatus Active = new ActiveStatus();
    public static readonly OfferStatus Deleted = new DeletedStatus();
    public static readonly OfferStatus Sold = new SoldStatus();
    public static readonly OfferStatus Draft = new DraftStatus();

    public abstract bool CanBeSold { get; }
    public abstract bool CanBeActivated { get; }

    private sealed class ActiveStatus() : OfferStatus("ACTIVE", 2)
    {
        public override bool CanBeSold => true;
        public override bool CanBeActivated => false;
    }

    private sealed class DeletedStatus() : OfferStatus("DELETED", 4)
    {
        public override bool CanBeSold => false;
        public override bool CanBeActivated => false;
    }

    private sealed class SoldStatus() : OfferStatus("SOLD", 5)
    {
        public override bool CanBeSold => false;
        public override bool CanBeActivated => false;
    }

    private sealed class DraftStatus() : OfferStatus("DRAFT", 6)
    {
        public override bool CanBeSold => false;
        public override bool CanBeActivated => true;
    }
}
