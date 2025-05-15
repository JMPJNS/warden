using NetCord.Rest;
using Warden.Bot.Extensions;
using Warden.Bot.Services;

namespace Warden.Bot.Modules.Scrim;

public class ScrimCommands(GuildConfigService gcs): ApplicationCommandModule<ApplicationCommandContext>
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

        if (!user.HasRole(guildConfig.TeamCaptainRoleId))
        {
            await ModifyResponseAsync(message => message.Content = "Ask your team captain to set up a scrim!");
            return;
        }
        
        await ModifyResponseAsync(message => message.Content = "Scrim Time");
    }
}