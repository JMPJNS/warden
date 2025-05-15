using Microsoft.EntityFrameworkCore;
using NetCord.Rest;

namespace Warden.Bot.Modules.Scrim;

public class ScrimCommands(GuildConfigService gcs, WardenDbContext db): ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("scrim", "Sign up for a scrim!", Contexts = [InteractionContextType.Guild])]
    public async Task Scrim([SlashCommandParameter(Description = "time in UTC (eg. 20:00 or 8PM)")] DateTimeOffset time)
    {
        var callback = InteractionCallback.DeferredMessage(MessageFlags.Ephemeral);
        await RespondAsync(callback);
        
        var guildConfig = await gcs.GetConfig(Context.Guild!.Id);
        if (guildConfig == null)
        {
            await ModifyResponseAsync(message => message.Content = "Guild is not configured");
            return;
        }
        
        var user = await Context.Client.Rest.GetGuildUserAsync(Context.Guild!.Id, Context.User.Id);
        
        var team = await db.Teams.FirstOrDefaultAsync(x => x.Players.Any(p => p.UserId == user.Id));
        if (team == null)
        {
            await ModifyResponseAsync(message => message.Content = "You are not part of a team");
            return;       
        }

        if (!user.HasRole(guildConfig.TeamCaptainRoleId))
        {
            await ModifyResponseAsync(message => message.Content = "Ask your team captain to set up a scrim!");
            return;
        }
        
        var scrim = new Data.Models.Scrim
        {
            Time = time,
            Team1 = team,
            Team2 = null,
            Ringers = []
        };
        
        await db.Scrims.AddAsync(scrim);
        await db.SaveChangesAsync();
        
        await Context.Client.Rest.SendMessageAsync(guildConfig.ScrimChannelId, ScrimMessageBuilder.Build(scrim));
        await ModifyResponseAsync(message => message.Content = "Scrim was set up");
    }
}