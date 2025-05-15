using Warden.Bot.Models;

namespace Warden.Bot.Services;

public class GuildConfigService
{
    public Task<GuildConfig?> GetConfig(ulong guildId)
    {
        var config = new GuildConfig
        {
            GuildId = guildId,
            ScrimChannelId = 1372311258363334776,
            TeamCaptainRoleId = 1372311911986761860
        };
        
        return Task.FromResult(config)!;
    }
}