namespace Dealmatcher.Backend.Domain;

public abstract class DealmatcherEntityBase : EntityBase
{
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }

    public abstract void Delete();
    public void MarkUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
