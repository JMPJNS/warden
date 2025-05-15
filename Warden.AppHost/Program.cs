using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDbGate();

builder.AddProject<Warden_Bot>("bot");

builder.Build().Run();