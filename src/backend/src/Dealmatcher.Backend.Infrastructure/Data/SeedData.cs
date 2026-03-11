namespace Dealmatcher.Backend.Infrastructure.Data;

public static class SeedData
{
    public static async Task InitializeAsync(AppDbContext dbContext)
    {
        await dbContext.SaveChangesAsync();
    }

    public static async Task InitializeTestAsync(AppDbContext dbContext)
    {
        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedOffers(AppDbContext dbContext)
    {
        await dbContext.SaveChangesAsync();
    }
}
