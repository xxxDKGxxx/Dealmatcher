using Dealmatcher.Backend.API.Middleware;

namespace Dealmatcher.Backend.API;

public sealed class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var logger = Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        logger.Information("Starting web host");

        builder.AddLoggerConfigs();

        var appLogger = new SerilogLoggerFactory(logger)
          .CreateLogger<Program>();

        try
        {
            builder.Services.AddServiceConfigs(appLogger, builder);
            builder.Services.AddAuthenticationConfigs(builder.Configuration);
            builder.Services.AddFastEndpoints()
                .SwaggerDocument(o =>
                    {
                        o.DocumentSettings = s =>
                            {
                                s.Title = "Dealmatcher API";
                                s.Version = "1";
                            };
                        o.ShortSchemaNames = true;
                        o.MaxEndpointVersion = 1;
                    });
            builder.Services.AddCommandMiddleware(c =>
            {
                c.Register(typeof(CommandLogger<,>));
            });

            var app = builder.Build();

            app.UseAuthentication();
            app.UseAuthorization();

            await app.UseAppMiddlewareAndSeedDatabase();

            await app.RunAsync();
        }
        catch (Exception ex)
        {
            logger.Error(ex, ex.Message);
            return;
        }
    }
}
