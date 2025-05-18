using Microsoft.EntityFrameworkCore;
using Warden.Data.Models;

namespace Warden.Data;

public class WardenDbContext: DbContext
{
    public DbSet<GuildConfig> GuildConfigs => Set<GuildConfig>();
    public DbSet<Scrim> Scrims => Set<Scrim>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Player> Players => Set<Player>();
    
    public WardenDbContext(DbContextOptions<WardenDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}