using Chronic.Core;
using Microsoft.EntityFrameworkCore;
using NetCord.Rest;

namespace Warden.Bot.Modules.Scrim;

public class ScrimCommands(GuildConfigService gcs, WardenDbContext db, PlayerService playerService): ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("scrim", "Sign up for a scrim!", Contexts = [InteractionContextType.Guild])]
    public async Task Scrim([SlashCommandParameter(Description = "eg. 20:00, in 2 hours, monday at 8pm...")] string time)
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
        var player = await playerService.GetPlayer(user.Id);

        if (player.Timezone is null)
        {
            await ModifyResponseAsync(message => message.Content = "Please set your timezone first with /setup timezone");
            return;
        }
        
        var tz = TimeZoneInfo.FindSystemTimeZoneById(player.Timezone);
        var nowInUserZone = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
        
        var parser = new Parser(new Options { Clock = () => nowInUserZone});
        var res = parser.Parse(time);
        if (res?.Start is null)
        {
            await ModifyResponseAsync(message => message.Content = "Could not parse time, please try a simpler expression.");
            return;
        }

        var localUserTime = DateTime.SpecifyKind(res.Start.Value, DateTimeKind.Unspecified);
        var utcTime = TimeZoneInfo.ConvertTimeToUtc(localUserTime, tz);
        
        var team = player?.Team;
        if (team == null)
        {
            await ModifyResponseAsync(message => message.Content = "You are not part of a team");
            return;       
        }

        if (!guildConfig.TeamCaptainRoleIds.Any(roleId => user.HasRole(roleId)))
        {
            await ModifyResponseAsync(message => message.Content = "Ask your team captain to set up a scrim!");
            return;
        }
        
        var scrim = new Data.Models.Scrim
        {
            Time = utcTime.ToUniversalTime(),
            Team1 = team,
            Team2 = null,
            Ringers = []
        };
        
        await db.Scrims.AddAsync(scrim);
        await db.SaveChangesAsync();

        var channelId = guildConfig.ScrimChannelId;

        // todo: make this configurable instead of hardcoding the channel ids
        if (Context.Guild.Id == 1214565847419457576)
        {
            // eu channel
            channelId = user.HasRole(1373769726701863062) ? (ulong)1373770032768745594 : (ulong)
                // na channel
                1364018922533425194;
        }
        
        var msg = await Context.Client.Rest.SendMessageAsync(channelId, ScrimMessageBuilder.Build(scrim));
        scrim.ScrimMsgId = msg.Id;
        await db.SaveChangesAsync();
        
        await ModifyResponseAsync(message => message.Content = "Scrim was set up");
    }
}