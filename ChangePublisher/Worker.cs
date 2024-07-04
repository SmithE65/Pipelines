using Microsoft.Data.SqlClient;

namespace ChangePublisher;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly SqlConnection _client;

    public Worker(ILogger<Worker> logger, SqlConnection client)
    {
        _logger = logger;
        _client = client;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            //using var scope = _serviceProvider.CreateScope();
            //var client = scope.ServiceProvider.GetRequiredService<SqlConnection>();
            var countCommand = new SqlCommand("SELECT COUNT(*) FROM InventoryItems", _client);
            var count = (int)await countCommand.ExecuteScalarAsync(stoppingToken);

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                _logger.LogInformation("Inventory item count: {count}", count);
            }

            await Task.Delay(10000, stoppingToken);
        }
    }
}
