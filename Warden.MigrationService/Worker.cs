using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Trace;
using Warden.Data;
using Warden.Data.Models;

public class Worker(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    public const string ActivitySourceName = "Migrations";
    private static readonly ActivitySource s_activitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var activity = s_activitySource.StartActivity("Migrating database", ActivityKind.Client);

        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<WardenDbContext>();

            await RunMigrationAsync(dbContext, cancellationToken);
            await SeedDataAsync(dbContext, cancellationToken);
        }
        catch (Exception ex)
        {
            activity?.RecordException(ex);
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }

    private static async Task RunMigrationAsync(WardenDbContext dbContext, CancellationToken cancellationToken)
    {
        Console.WriteLine("Available migrations:");
        var migrations = dbContext.Database.GetMigrations();
        foreach (var migration in migrations)
        {
            Console.WriteLine($" - {migration}");
        }

        
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Run migration in a transaction to avoid partial migration if it fails.
            await dbContext.Database.MigrateAsync(cancellationToken);
        });
    }

    private static async Task SeedDataAsync(WardenDbContext dbContext, CancellationToken cancellationToken)
    {
        var herbertConfig = new GuildConfig
        {
            GuildId = 1372282859796037766,
            ScrimChannelId = 1372311258363334776,
            TeamCaptainRoleId = 1372311911986761860
        };
        
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Seed the database
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            await dbContext.GuildConfigs.AddAsync(herbertConfig, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        });
    }
}