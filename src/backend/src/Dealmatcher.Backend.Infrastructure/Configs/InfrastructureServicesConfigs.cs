using Dealmatcher.Backend.Domain.Interfaces.Payment;
using Dealmatcher.Backend.Infrastructure.Services.CartRepositories;
using Dealmatcher.Backend.Infrastructure.Services.PaymentProviders;
using StackExchange.Redis;

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
            .AddScoped<ITokenService, JwtTokenService>()
            .AddScoped<IImageStorageService, AzureBlobStorageService>()
            .AddScoped<IOfferSuggestionService, RandomOfferSuggestionService>();

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var connectionString = config.GetValue<string>("ConnectionStrings:Redis")
                ?? throw new Exception("ConnectionStrings:Redis not configured");
            return ConnectionMultiplexer.Connect(connectionString);
        });

        services.AddScoped<ICartRepository, RedisCartRepository>();

        services.AddScoped<IPaymentProvider, ExamplePaymentProvider>();
        services.AddScoped<IPaymentProviderService, PaymentProviderService>();

        logger.LogInformation("{Project} services registered.", "Infrastructure");

        return services;
    }
}
