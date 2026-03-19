namespace Dealmatcher.Backend.Infrastructure.Data.Interceptors;

public sealed class SoftDeleteInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
         DbContextEventData eventData,
         InterceptionResult<int> result,
         CancellationToken cancellationToken = default)
    {
        ProcessSoftDelete(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ProcessSoftDelete(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    private void ProcessSoftDelete(DbContext? dbContext)
    {
        if (dbContext is null)
        {
            return;
        }

        var entries = dbContext.ChangeTracker.Entries()
            .Where(e => e is { State: EntityState.Deleted, Entity: DealmatcherEntityBase })
            .ToList();

        if (entries.Count == 0)
        {
            return;
        }

        foreach (var entityEntry in entries)
        {
            entityEntry.State = EntityState.Modified;

            var dealmatcherEntity = entityEntry.Entity as DealmatcherEntityBase;

            dealmatcherEntity?.Delete();
        }
    }
}
