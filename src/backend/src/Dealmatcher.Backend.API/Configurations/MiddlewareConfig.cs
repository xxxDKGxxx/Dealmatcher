namespace Dealmatcher.Backend.API.Configurations;

public static class MiddlewareConfig
{
    public static async Task<IApplicationBuilder> UseAppMiddlewareAndSeedDatabase(
      this WebApplication app
    )
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

        var allowedUrls = app.Configuration.GetSection("AllowedUrls").Get<string[]>();

        if (allowedUrls is not null)
        {
            app.UseCors(opt =>
            {
                opt.WithOrigins(allowedUrls).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
            });
        }

        app.UseFastEndpoints(c =>
          {
              c.Endpoints.RoutePrefix = "api";
              c.Versioning.Prefix = "v";
              c.Versioning.PrependToRoute = true;
          })
          .UseSwaggerGen(); // Includes AddFileServer and static files middleware

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
