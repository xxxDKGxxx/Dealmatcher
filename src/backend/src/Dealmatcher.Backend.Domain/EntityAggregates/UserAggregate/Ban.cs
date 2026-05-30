namespace Dealmatcher.Backend.Domain.EntityAggregates.UserAggregate;

public class Ban : DealmatcherEntityBase
{
    public int UserId { get; private set; }
    public string Reason { get; private set; } = null!;
    public int IssuedById { get; private set; }
    public DateTime IssuedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public bool IsActive { get; private set; }

    private Ban() { }

    internal Ban(int userId, string reason, int issuedById, DateTime? expiresAt)
    {
        UserId = userId;
        Reason = reason;
        IssuedById = issuedById;
        IssuedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
        IsActive = true;
    }

    internal void Revoke()
    {
        IsActive = false;
    }
}
