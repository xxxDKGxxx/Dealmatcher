namespace Dealmatcher.Backend.Infrastructure.Data.Interceptors;

public sealed class UpdateTimestampInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateModificationDate(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateModificationDate(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    private static void UpdateModificationDate(DbContext? dbContext)
    {
        if (dbContext is null)
        {
            return;
        }

        var entries = dbContext.ChangeTracker.Entries()
            .Where(e => e is { State: EntityState.Modified, Entity: DealmatcherEntityBase })
            .ToList();

        if (entries.Count == 0)
        {
            return;
        }

        foreach (var entityEntry in entries)
        {
            var dealmatcherEntity = entityEntry.Entity as DealmatcherEntityBase;
            dealmatcherEntity?.MarkUpdated();
        }
    }
}
