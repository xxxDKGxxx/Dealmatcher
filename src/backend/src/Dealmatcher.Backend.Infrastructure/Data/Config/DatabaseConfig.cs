namespace Dealmatcher.Backend.Infrastructure.Data.Config;

public static class DatabaseConfig
{
    public static void AddApplicationDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null
                );
            });
        });
    }
}
