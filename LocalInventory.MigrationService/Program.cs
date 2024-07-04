using LocalInventory.Api.Data;
using LocalInventory.MigrationService;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();
builder.AddSqlServerDbContext<InventoryDbContext>("local-inventory");

var host = builder.Build();
host.Run();
