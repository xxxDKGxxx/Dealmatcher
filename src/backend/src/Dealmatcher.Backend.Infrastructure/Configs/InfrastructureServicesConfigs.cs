namespace Dealmatcher.Backend.Infrastructure.Configs;

public static class InfrastructureServicesConfigs
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        ConfigurationManager config,
        Microsoft.Extensions.Logging.ILogger logger)
    {
        string? connectionString;

        try
        {
            connectionString = config.GetConnectionString("DefaultConnection");
            Guard.Against.Null(connectionString);
        }
        catch
        {
            logger.LogError("Default connection string was not defined in the environment");
            throw;
        }

        services.AddApplicationDbContext(connectionString);
        services.AddAutoMapperConfigs();

        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>))
            .AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));

        logger.LogInformation("{Project} services registered.", "Infrastructure");

        return services;
    }
}
