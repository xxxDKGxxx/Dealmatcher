namespace Dealmatcher.Backend.Infrastructure.Data;

public static class SeedData
{
    public static async Task InitializeAsync(AppDbContext dbContext)
    {
        await dbContext.SaveChangesAsync();
    }

    public static async Task InitializeTestAsync(AppDbContext dbContext)
    {
        var ex = new Example(10);
        if (!dbContext.Set<Example>().Any()) 
        {
            await dbContext.AddAsync(ex);    
        }

        await dbContext.SaveChangesAsync();
    }
}
