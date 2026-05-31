namespace Dealmatcher.Backend.API.Configurations;

public static class ServiceConfigs
{
    public static IServiceCollection AddServiceConfigs(this IServiceCollection services, Microsoft.Extensions.Logging.ILogger logger, WebApplicationBuilder builder)
    {
        services.AddInfrastructureServices(builder.Configuration, logger, builder.Environment.IsProduction())
            .AddMediatrConfigs();

        services.AddHostedService<BanExpirationWorker>();

        logger.LogInformation("{Project} services registered", "Mediatr, AutoMapper, Background Workers");

        return services;
    }
}
