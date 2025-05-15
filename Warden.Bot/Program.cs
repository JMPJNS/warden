global using NetCord.Services.ApplicationCommands;
global using NetCord.Services.ComponentInteractions;
global using NetCord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Hosting.Services.ComponentInteractions;
using Warden.Bot;
using Warden.Bot.Services;
using Warden.Bot.TypeReaders;
using Warden.ServiceDefaults;


var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddMemoryCache()
    .AddLogging();

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<WardenDbContext>("postgres");

// warden services
builder.Services.AddScoped<GuildConfigService>();

// discord services
builder.Services
    .AddDiscordGateway(o => o.Token = Environment.GetEnvironmentVariable("BOT_TOKEN"))
    .AddApplicationCommands(o => 
        o.TypeReaders.Add(typeof(DateTimeOffset), new DateTimeOffsetTypeReader<ApplicationCommandContext>()))
    .AddComponentInteractions<ButtonInteraction, ButtonInteractionContext>();

var host = builder.Build();

host.AddModules(typeof(Program).Assembly);
host.UseGatewayEventHandlers();

await host.RunAsync();