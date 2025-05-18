namespace Warden.Bot.Modules.Setup;

[SlashCommand("setup", "Setup commands")]
public class SetupCommands(WardenDbContext db, PlayerService playerService): ApplicationCommandModule<ApplicationCommandContext>
{
    [SubSlashCommand("profile", "Configure your steam profile")]
    public async Task Profile(string steamProfile)
    {
        var callback = InteractionCallback.DeferredMessage(MessageFlags.Ephemeral);
        await RespondAsync(callback);
        
        var player = await playerService.GetPlayer(Context.User.Id);
        await playerService.SetSteamProfile(player, steamProfile);
        
        await ModifyResponseAsync(message => message.Content = "Done");
    }

    [SubSlashCommand("timezone", "Configure your timezone")]
    public async Task Timezone([SlashCommandParameter(AutocompleteProviderType = typeof(TimeZoneAutocompleteProvider), Description = "try typing your capital's name (e.g. Europe/Vienna, UTC+02, Central European Time)")] string timezone)
    {
        var callback = InteractionCallback.DeferredMessage(MessageFlags.Ephemeral);
        await RespondAsync(callback);
        
        var player = await playerService.GetPlayer(Context.User.Id);
        await playerService.SetTimezone(player, timezone);
        
        await ModifyResponseAsync(message => message.Content = $"`{timezone}` set up");
    }
    
    public class TimeZoneAutocompleteProvider : IAutocompleteProvider<AutocompleteInteractionContext>
    {
        public async ValueTask<IEnumerable<ApplicationCommandOptionChoiceProperties>?> GetChoicesAsync(ApplicationCommandInteractionDataOption option, AutocompleteInteractionContext context)
        {
            var timezones = TimeZoneInfo.GetSystemTimeZones()
                .Where(x =>
                    x.Id.Contains(option.Value ?? string.Empty, StringComparison.InvariantCultureIgnoreCase)
                    || x.DisplayName.Contains(option.Value ?? string.Empty, StringComparison.InvariantCultureIgnoreCase))
                .OrderBy(x => x.Id)
                .Take(10)
                .Select(x => new ApplicationCommandOptionChoiceProperties(x.DisplayName, x.Id));
            
            return timezones;
        }
    }
}