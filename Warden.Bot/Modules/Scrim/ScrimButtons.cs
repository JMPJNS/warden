using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetCord.Rest;

namespace Warden.Bot.Modules.Scrim;

public class ScrimButtons(WardenDbContext db, GuildConfigService gcs, ILogger<ScrimButtons> logger): ComponentInteractionModule<ButtonInteractionContext>
{
    [ComponentInteraction("join scrim")]
    public async Task JoinScrim(int scrimId)
    {
        var callback = InteractionCallback.DeferredMessage(MessageFlags.Ephemeral);
        await RespondAsync(callback);
        
        var scrim = db.Scrims
            .Include(x => x.Team1)
            .Include(x => x.Team2)
            .FirstOrDefault(x => x.Id == scrimId);

        if (scrim is null)
        {
            var errorMessage = $"Scrim with ID: {scrimId} doesn't exist";
            logger.LogError(errorMessage);
            await ModifyResponseAsync(message => message.Content = errorMessage);
            return;
        }

        if (scrim.Team2 is not null)
        {
            await ModifyResponseAsync(message => message.Content = "Scrim is already filled, try setting up a new scrim with /scrim");
            return;
        }
        
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

        if (scrim.Team1.Id == team.Id)
        {
            await ModifyResponseAsync(message => message.Content = "You can't sign up for your own scrim");
            return;    
        }

        if (!user.HasRole(guildConfig.TeamCaptainRoleId))
        {
            await ModifyResponseAsync(message => message.Content = "Ask your team captain to set up a scrim");
            return;
        }
        
        scrim.Team2 = team;
        await db.SaveChangesAsync();
        
        await UpdateScrimMessage(scrim);
        
        await ModifyResponseAsync(message => message.Content = "Signed up");
    }

    [ComponentInteraction("scrim looking ringer")]
    public async Task LookForRinger(int scrimId)
    {
        var callback = InteractionCallback.DeferredMessage(MessageFlags.Ephemeral);
        await RespondAsync(callback);
        
        var scrim = db.Scrims
            .FirstOrDefault(x => x.Id == scrimId);
        
        if (scrim is null)
        {
            var errorMessage = $"Scrim with ID: {scrimId} doesn't exist";
            logger.LogError(errorMessage);
            await ModifyResponseAsync(message => message.Content = errorMessage);
            return;
        }

        var guildConfig = await gcs.GetConfig(Context.Guild!.Id);

        if (guildConfig is null)
        {
            await ModifyResponseAsync(message => message.Content = "Guild is not configured");
            return;
        }
        
        var msg = await Context.Client.Rest.SendMessageAsync(guildConfig.RingerChannelId, ScrimMessageBuilder.BuildRingerMessage(scrim));
        scrim.RingerMsgId = msg.Id;
        await db.SaveChangesAsync();
        
        await ModifyResponseAsync(message => message.Content = "Sent");
    }

    [ComponentInteraction("scrim ringer")]
    public async Task JoinScrimAsRinger(int scrimId)
    {
        var callback = InteractionCallback.DeferredMessage(MessageFlags.Ephemeral);
        await RespondAsync(callback);
        
        var scrim = db.Scrims
            .Include(x => x.Team1)
            .Include(x => x.Team2)
            .FirstOrDefault(x => x.Id == scrimId);

        if (scrim is null)
        {
            var errorMessage = $"Scrim with ID: {scrimId} doesn't exist";
            logger.LogError(errorMessage);
            await ModifyResponseAsync(message => message.Content = errorMessage);
            return;
        }

        if (scrim.Cancelled)
        {
            await ModifyResponseAsync(message => message.Content = "Scrim was cancelled");
            return;
        }

        // first check if user is already signed up as ringer
        if (scrim.Ringers.Contains(Context.User.Id))
        {
            scrim.Ringers.Remove(Context.User.Id);
            await db.SaveChangesAsync();
            
            await UpdateScrimMessage(scrim);
            
            await ModifyResponseAsync(message => message.Content = "Left scrim as ringer");
            return;
        }
        
        scrim.Ringers.Add(Context.User.Id);
        await db.SaveChangesAsync();
        
        await UpdateScrimMessage(scrim);
        
        await ModifyResponseAsync(message => message.Content = $"Signed up, be there at <t:{scrim.Time.ToUnixTime()}>! Click the button again to leave");
    }

    [ComponentInteraction("scrim cancel")]
    public async Task CancelScrim(int scrimId)
    {
        var callback = InteractionCallback.DeferredMessage(MessageFlags.Ephemeral);
        await RespondAsync(callback);
        
        var scrim = db.Scrims
            .Include(x => x.Team1).ThenInclude(team => team.Players)
            .Include(x => x.Team2).ThenInclude(team => team!.Players)
            .FirstOrDefault(x => x.Id == scrimId);

        if (scrim is null)
        {
            var errorMessage = $"Scrim with ID: {scrimId} doesn't exist";
            logger.LogError(errorMessage);
            await ModifyResponseAsync(message => message.Content = errorMessage);
            return;
        }
        
        var guildConfig = await gcs.GetConfig(Context.Guild!.Id);
        if (guildConfig == null)
        {
            await ModifyResponseAsync(message => message.Content = "Guild is not configured");
            return;
        }
        
        var player = await db.Players.FirstOrDefaultAsync(x => x.UserId == Context.User.Id);
        if (player is null)
        {
            await ModifyResponseAsync(message => message.Content = "You are not part of the scrim");
            return;
        }
        
        var team = await db.Teams.FirstOrDefaultAsync(x => x.Players.Any(p => p.Id == player.Id));
        if (team == null)
        {
            await ModifyResponseAsync(message => message.Content = "You are not part of a team");
            return;       
        }
        
        var user = await Context.Client.Rest.GetGuildUserAsync(Context.Guild!.Id, Context.User.Id);
        if (!user.HasRole(guildConfig.TeamCaptainRoleId))
        {
            await ModifyResponseAsync(message => message.Content = "Ask your team captain to cancel");
            return;
        }

        if (scrim.Team1?.Players.Contains(player) == true)
        {
            scrim.Cancelled = true;
            await db.SaveChangesAsync();
            await UpdateScrimMessage(scrim);
            
            await ModifyResponseAsync(message => message.Content = "Cancelled the scrim");
            // TODO notify the other participants

            return;
        }

        if (scrim.Team2?.Players.Contains(player) == true)
        {
            scrim.Team2 = null;
            await db.SaveChangesAsync();
            await UpdateScrimMessage(scrim);
            
            await ModifyResponseAsync(message => message.Content = "Cancelled the scrim");
            // TODO notify the other team
        }
    }
    
    private async Task UpdateScrimMessage(Data.Models.Scrim scrim)
    {
        var guildConfig = await gcs.GetConfig(Context.Guild!.Id);
        if (guildConfig is null)
        {
            await ModifyResponseAsync(message => message.Content = "Guild is not configured");
            return;
        }
        
        var msgId = scrim.ScrimMsgId;

        if (msgId is null)
        {
            await ModifyResponseAsync(message => message.Content = $"Scrim message for ID: {scrim.Id} not found");
            return;       
        }
        
        var channelId = guildConfig.ScrimChannelId;
        
        await Context.Client.Rest.ModifyMessageAsync(channelId, (ulong)msgId, m =>
        {
            var scrimMsg = ScrimMessageBuilder.Build(scrim);
            m.Content = scrimMsg.Content;
            m.Embeds = scrimMsg.Embeds;
            m.Components = scrimMsg.Components ?? [];
            m.AllowedMentions = scrimMsg.AllowedMentions;
        });
    }
}