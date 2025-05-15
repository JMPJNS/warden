using Microsoft.EntityFrameworkCore;
using Warden.Bot.Models;

namespace Warden.Bot;

public class WardenDbContext: DbContext
{
    public DbSet<GuildConfig> GuildConfigs { get; set; }
    
    public WardenDbContext(DbContextOptions<WardenDbContext> options)
        : base(options)
    {
    }
}