var builder = DistributedApplication.CreateBuilder(args);

var localSql = builder.AddSqlServer("local-inventory-service");
localSql.WithEnvironment("MSSQL_AGENT_ENABLED", "true");

var localSqlDb = localSql.AddDatabase("local-inventory");

builder.AddProject<Projects.Mapper>("mapper");

builder.AddProject<Projects.LocalInventory_Api>("localinventory")
    .WithReference(localSqlDb);

builder.AddProject<Projects.GlobalInventory>("globalinventory");

var migrationService = builder.AddProject<Projects.LocalInventory_MigrationService>("localinventory-migrationservice")
    .WithReference(localSqlDb);

builder.AddProject<Projects.ChangePublisher>("changepublisher")
    .WithReference(localSqlDb)
    .WithReference(migrationService);

builder.Build().Run();
