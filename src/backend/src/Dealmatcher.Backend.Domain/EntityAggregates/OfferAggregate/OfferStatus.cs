namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate;

public abstract class OfferStatus(string name, int value) : SmartEnum<OfferStatus>(name, value)
{
    public static readonly OfferStatus Draft = new DraftStatus();
    public static readonly OfferStatus Active = new DraftStatus();
    public static readonly OfferStatus Promoted = new DraftStatus();
    public static readonly OfferStatus Deleted = new DraftStatus();
    public static readonly OfferStatus Sold = new DraftStatus();

    public abstract bool CanBeSold { get; }
    public abstract bool CanBePromoted { get; }

    private sealed class DraftStatus() : OfferStatus("DRAFT", 1)
    {
        public override bool CanBeSold => false;
        public override bool CanBePromoted => false;
    }

    private sealed class ActiveStatus() : OfferStatus("ACTIVE", 2)
    {
        public override bool CanBeSold => true;
        public override bool CanBePromoted => true;
    }

    private sealed class PromotedStatus() : OfferStatus("PROMOTED", 3)
    {
        public override bool CanBeSold => true;
        public override bool CanBePromoted => false;
    }

    private sealed class DeletedStatus() : OfferStatus("DELETED", 4)
    {
        public override bool CanBeSold => false;
        public override bool CanBePromoted => false;
    }

    private sealed class SoldStatus() : OfferStatus("SOLD", 5)
    {
        public override bool CanBeSold => false;
        public override bool CanBePromoted => false;
    }
}
