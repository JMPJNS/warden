using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder
    .AddPostgres("postgres")
    .WithDbGate()
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("wardendb");

builder.AddProject<Warden_Bot>("bot")
    .WaitFor(postgres)
    .WithReference(postgres);

builder.Build().Run();