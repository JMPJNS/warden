using Warden.Data;
using Warden.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddHostedService<Worker>();
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));
builder.AddNpgsqlDbContext<WardenDbContext>("postgres");

var host = builder.Build();
host.Run();
