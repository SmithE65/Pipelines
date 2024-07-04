using ChangePublisher;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();
builder.AddSqlServerClient("local-inventory");

var host = builder.Build();
host.Run();
