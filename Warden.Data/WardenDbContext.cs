using Microsoft.EntityFrameworkCore;
using Warden.Data.Models;

namespace Warden.Data;

public class WardenDbContext: DbContext
{
    public DbSet<GuildConfig> GuildConfigs => Set<GuildConfig>();
    
    public WardenDbContext(DbContextOptions<WardenDbContext> options)
        : base(options)
    {
    }
}