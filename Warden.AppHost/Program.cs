using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder
    .AddPostgres("postgres", port: 5437)
    .WithEndpoint(name: "postgresEndpoint", targetPort: 5437)
    .WithDbGate()
    .WithLifetime(ContainerLifetime.Persistent);


var migrations = builder.AddProject<Warden_MigrationService>("migrations")
    .WithReference(postgres)
    .WaitFor(postgres);

builder.AddProject<Warden_Bot>("bot")
    .WaitForCompletion(migrations)
    .WithReference(postgres);

builder.Build().Run();