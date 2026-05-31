namespace Dealmatcher.Backend.Domain.EntityAggregates.UserAggregate;

public class Ban : DealmatcherEntityBase
{
    public User User { get; private set; } = null!;
    public string Reason { get; private set; } = null!;
    public User IssuedBy { get; private set; } = null!;
    public DateTime IssuedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public bool IsActive { get; private set; }

    /* EF Core*/
    private Ban() { }

    internal Ban(User user, string reason, User issuedBy, DateTime? expiresAt)
    {
        User = user;
        Reason = reason;
        IssuedBy = issuedBy;
        IssuedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
        IsActive = true;
    }

    internal void Revoke()
    {
        IsActive = false;
    }
}
