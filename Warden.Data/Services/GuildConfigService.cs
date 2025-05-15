using Microsoft.EntityFrameworkCore;
using Warden.Data.Models;

namespace Warden.Data.Services;

public class GuildConfigService(WardenDbContext db)
{
    public Task<GuildConfig?> GetConfig(ulong guildId, CancellationToken ct = default)
    {
        return db.GuildConfigs.FirstOrDefaultAsync(x => x.GuildId == guildId, ct);
    }
}