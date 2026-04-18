namespace Dealmatcher.Backend.Domain;

public abstract class DealmatcherEntityBase : EntityBase
{
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }
    public bool IsDeleted { get; private set; } = false;

    public virtual void Delete()
    {
        IsDeleted = true;
    }

    public virtual void UnDelete()
    {
        IsDeleted = false;
    }

    public void MarkUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
