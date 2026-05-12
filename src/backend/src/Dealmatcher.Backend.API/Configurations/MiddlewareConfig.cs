namespace Dealmatcher.Backend.API.Configurations;

public static class MiddlewareConfig
{
    public static async Task<IApplicationBuilder> UseAppMiddlewareAndSeedDatabase(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseShowAllServicesMiddleware(); // see https://github.com/ardalis/AspNetCoreStartupServices
        }
        else
        {
            app.UseDefaultExceptionHandler(); // from FastEndpoints
            app.UseHsts();
        }

        app.UseFastEndpoints(
                c =>
                {
                    c.Endpoints.RoutePrefix = "api";
                    c.Versioning.Prefix = "v";
                    c.Versioning.PrependToRoute = true;
                })
            .UseSwaggerGen(); // Includes AddFileServer and static files middleware

        var frontendOrigin = app.Configuration["FrontendOrigin"];

        if (frontendOrigin is not null)
        {
            app.UseCors(opt =>
            {
                opt.WithOrigins(frontendOrigin)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        } // else we are in prod environment and FrontendOrigin is not set

        await SeedDatabase(app);

        return app;
    }

    private static async Task SeedDatabase(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var context = services.GetRequiredService<AppDbContext>();
            if (context.Database.IsRelational())
            {
                await context.Database.MigrateAsync();
            }
            else
            {
                await context.Database.EnsureCreatedAsync();
            }

            if (app.Environment.IsDevelopment())
            {
                var storageService = services.GetService<IImageStorageService>();
                await SeedData.InitializeTestAsync(context, storageService);
            }
            else
            {
                await SeedData.InitializeAsync(context);
            }
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred seeding the DB. {exceptionMessage}", ex.Message);
        }
    }
}
