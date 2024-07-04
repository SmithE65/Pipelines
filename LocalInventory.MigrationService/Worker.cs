using LocalInventory.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace LocalInventory.MigrationService;

public class Worker(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IHostApplicationLifetime _hostApplicationLifetime = hostApplicationLifetime;
    private static readonly ActivitySource _activitySource = new("LocalInventory.MigrationService");

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var activity = _activitySource.StartActivity("Migrating database", ActivityKind.Client);

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();

            await EnsureDatabaseCreatedAsync(dbContext, stoppingToken);
            await RunMigrationsAsync(dbContext, stoppingToken);
            await SeedDataAsync(dbContext, stoppingToken);
        }
        catch (Exception ex)
        {
            activity?.RecordException(ex);
            throw;
        }

        _hostApplicationLifetime.StopApplication();
    }

    private static async Task EnsureDatabaseCreatedAsync(InventoryDbContext dbContext, CancellationToken cancellationToken)
    {
        var dbCreator = dbContext.GetService<IRelationalDatabaseCreator>();
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async (ct) =>
        {
            if (!await dbCreator.ExistsAsync(ct))
            {
                await dbCreator.CreateAsync(ct);
            }
        }, cancellationToken);
    }

    private static async Task RunMigrationsAsync(InventoryDbContext dbContext, CancellationToken cancellation)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async (ct) =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);
            await dbContext.Database.MigrateAsync(ct);
            await transaction.CommitAsync(ct);
        }, cancellation);
    }

    private static async Task SeedDataAsync(InventoryDbContext dbContext, CancellationToken cancellationToken)
    {
        var parentRack = new Location { Name = "A001", Description = "Rack" };
        Location[] locations = 
        [
            parentRack,
            new Location { Name = "A001-01", Description = "Shelf", Parent = parentRack },
            new Location { Name = "A001-02", Description = "Shelf", Parent = parentRack },
            new Location { Name = "A001-03", Description = "Shelf", Parent = parentRack },
        ];

        Product[] products =
        [
            new Product { Name = "Product 1", Description = "Product 1 Description" },
            new Product { Name = "Product 2", Description = "Product 2 Description" },
            new Product { Name = "Product 3", Description = "Product 3 Description" },
        ];

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async (ct) =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);
            await dbContext.Locations.AddRangeAsync(locations, ct);
            await dbContext.Products.AddRangeAsync(products, ct);
            await dbContext.SaveChangesAsync(ct);

            await transaction.CommitAsync(ct);
        }, cancellationToken);
    }
}
