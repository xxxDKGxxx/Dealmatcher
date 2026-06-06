using Dealmatcher.Backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace Dealmatcher.Backend.IntegrationTests.Infrastructure.Data;

public class MigrationsTests : IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04").Build();

    public async Task InitializeAsync() => await _dbContainer.StartAsync();

    public async Task DisposeAsync() => await _dbContainer.DisposeAsync();

    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(_dbContainer.GetConnectionString())
            .Options;

        return new AppDbContext(options, dispatcher: null);
    }

    [Fact]
    public async Task AllMigrations_ApplyCleanlyToRealDatabase()
    {
        await using var db = CreateDbContext();

        await Should.NotThrowAsync(() => db.Database.MigrateAsync());

        var pending = await db.Database.GetPendingMigrationsAsync();
        pending.ShouldBeEmpty();
    }
}
