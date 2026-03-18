namespace Dealmatcher.Backend.Domain;

public class DealmatcherEntityBase : EntityBase
{
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
}
