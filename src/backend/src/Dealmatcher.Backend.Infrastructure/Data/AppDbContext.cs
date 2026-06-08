namespace Dealmatcher.Backend.Infrastructure.Data;

public sealed class AppDbContext(
    DbContextOptions<AppDbContext> options,
    IDomainEventDispatcher? dispatcher) : DbContext(options)
{
    private readonly IDomainEventDispatcher? _dispatcher = dispatcher;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        int result;
        try
        {
            result = await base.SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException("Resource was modified by another user.", ex);
        }

        // ignore events if no dispatcher provided
        if (_dispatcher == null)
        {
            return result;
        }

        // dispatch events only if save was successful
        var entitiesWithEvents = ChangeTracker.Entries<HasDomainEventsBase>()
            .Select(e =>
            {
                return e.Entity;
            })
            .Where(e =>
            {
                return e.DomainEvents.Count != 0;
            })
            .ToArray();

        await _dispatcher.DispatchAndClearEvents(entitiesWithEvents);

        return result;
    }

    public override int SaveChanges()
    {
        var result = SaveChangesAsync().
            GetAwaiter().
            GetResult();
        return result;
    }
}
