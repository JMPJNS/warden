using Microsoft.EntityFrameworkCore;
using Warden.Data.Models;

namespace Warden.Data.Services;

public class PlayerService(WardenDbContext db)
{
    /// <summary>
    /// Find a player by their discord user id
    /// </summary>
    /// <param name="userId">Discord user Id</param>
    /// <returns>Existing Player if found, otherwise creates a new one</returns>
    public async Task<Player> GetPlayer(ulong userId, CancellationToken ct = default)
    {
        var found = await db.Players
            .Include(x => x.Team)
            .FirstOrDefaultAsync(x => x.UserId == userId, ct);
        if (found is not null)
        {
            return found;
        }
        
        var player = new Player
        {
            UserId = userId,
        };
        db.Players.Add(player);
        await db.SaveChangesAsync(ct);
        return player;
    }

    public async Task SetSteamProfile(Player player, string steamProfile, CancellationToken ct = default)
    {
        player.SteamProfile = steamProfile;
        await db.SaveChangesAsync(ct);
    }

    public async Task SetTimezone(Player player, string timezone, CancellationToken ct = default)
    {
        player.Timezone = timezone;
        await db.SaveChangesAsync(ct);
    }
}