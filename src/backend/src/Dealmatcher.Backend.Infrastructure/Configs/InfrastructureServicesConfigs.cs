namespace Dealmatcher.Backend.Infrastructure.Configs;

public static class InfrastructureServicesConfigs
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration config,
        Microsoft.Extensions.Logging.ILogger logger,
        bool isProduction = false)
    {
        string? connectionString = config.GetConnectionString("DefaultConnection");

        if (isProduction && string.IsNullOrWhiteSpace(connectionString))
        {
            logger.LogError("Default connection string was not defined in the environment");
            throw new InvalidOperationException("Default connection string was not defined in the environment");
        }

        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            services.AddApplicationDbContext(connectionString);
        }

        services.AddAutoMapperConfigs();

        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>))
            .AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>))
            .AddScoped<IPasswordHasher, BCryptPasswordHasher>()
            .AddScoped<ITokenService, JwtTokenService>();

        logger.LogInformation("{Project} services registered.", "Infrastructure");

        return services;
    }
}
