namespace Dealmatcher.Backend.Domain;

public abstract class DealmatcherEntityBase : EntityBase
{
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }
    public bool IsDeleted { get; private set; } = false;

    public virtual void Delete()
    {
        IsDeleted = false;
    }

    public virtual void UnDelete()
    {
        IsDeleted = true;
    }

    public void MarkUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
