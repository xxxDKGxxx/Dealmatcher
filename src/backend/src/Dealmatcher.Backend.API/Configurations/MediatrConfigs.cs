namespace Dealmatcher.Backend.API.Configurations;

public static class MediatrConfigs
{
    public static IServiceCollection AddMediatrConfigs(this IServiceCollection services)
    {
        var mediatRAssemblies = new[]
        {
            Assembly.GetAssembly(typeof(Program)),
            Assembly.GetAssembly(typeof(LoginCommandHandler))
        };

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(mediatRAssemblies!);
        })
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
            .AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();

        return services;
    }
}
