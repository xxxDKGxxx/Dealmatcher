namespace Dealmatcher.Backend.Infrastructure.Configs;

public static class AutoMapperConfig
{
    public static IServiceCollection AddAutoMapperConfigs(this IServiceCollection services)
    {
        services.AddAutoMapper(
            _ =>
            {

            },
            typeof(ExampleProfile).Assembly);

        return services;
    }
}
