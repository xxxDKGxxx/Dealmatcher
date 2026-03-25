namespace Dealmatcher.Backend.Domain.EntityAggregates.UserAggregate;

public abstract class UserStatus(string name, int value) : SmartEnum<UserStatus>(name, value)
{
    public static readonly UserStatus Active = new ActiveStatus();
    public static readonly UserStatus Inactive = new InactiveStatus();
    public static readonly UserStatus Banned = new BannedStatus();

    public abstract bool CanLogin { get; }

    private sealed class ActiveStatus() : UserStatus("ACTIVE", 1)
    {
        public override bool CanLogin => true;
    }

    private sealed class InactiveStatus() : UserStatus("INACTIVE", 2)
    {
        public override bool CanLogin => false;
    }

    private sealed class BannedStatus() : UserStatus("BANNED", 3)
    {
        public override bool CanLogin => false;
    }
}
