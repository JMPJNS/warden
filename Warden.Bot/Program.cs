global using NetCord.Services.ApplicationCommands;
global using NetCord.Services.ComponentInteractions;
global using NetCord;
global using NetCord.Rest;
global using Warden.Data;
global using Warden.Data.Models;
global using Warden.Bot.Extensions;
global using Warden.Data.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Hosting.Services.ComponentInteractions;
using Warden.Bot.TypeReaders.SlashCommand;
using Warden.ServiceDefaults;


var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddMemoryCache()
    .AddLogging();

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<WardenDbContext>("postgres");

// warden services
builder.Services.AddScoped<GuildConfigService>();
builder.Services.AddScoped<PlayerService>();

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