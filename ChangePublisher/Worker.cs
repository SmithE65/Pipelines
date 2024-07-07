using Microsoft.Data.SqlClient;

namespace ChangePublisher;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var client = scope.ServiceProvider.GetRequiredService<SqlConnection>();
                await client.OpenAsync(stoppingToken);
                var countCommand = new SqlCommand("SELECT COUNT(*) FROM Inventory", client);
                var count = (int)await countCommand.ExecuteScalarAsync(stoppingToken);

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    _logger.LogInformation("Inventory item count: {count}", count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong.");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
