namespace Dealmatcher.Backend.API.Workers;

public class BanExpirationWorker(
    IServiceProvider serviceProvider,
    ILogger<BanExpirationWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Ban Expiration Worker has started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

                await mediator.Send(new RevokeExpiredBansCommand(), stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while revoking expired bans.");
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
