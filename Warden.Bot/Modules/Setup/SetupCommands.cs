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

        // check if its a valid timezone
        if (TimeZoneInfo.GetSystemTimeZones().All(x => x.Id != timezone))
        {
            await ModifyResponseAsync(message => message.Content = "Invalid timezone selected, choose from the autocomplete options");
            return;       
        }
        
        var player = await playerService.GetPlayer(Context.User.Id);
        await playerService.SetTimezone(player, timezone);
        
        await ModifyResponseAsync(message => message.Content = $"`{timezone}` set up");
    }
    
    public class TimeZoneAutocompleteProvider : IAutocompleteProvider<AutocompleteInteractionContext>
    {
        public async ValueTask<IEnumerable<ApplicationCommandOptionChoiceProperties>?> GetChoicesAsync(ApplicationCommandInteractionDataOption option, AutocompleteInteractionContext context)
        {
            var timezones = TimeZoneInfo.GetSystemTimeZones()
                .Select(tz => new
                {
                    tz,
                    StdAbbrev = string.Concat(
                        tz.StandardName
                            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                            .Select(w => w[0])),
                    DstAbbrev = string.Concat(
                        tz.DaylightName
                            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                            .Select(w => w[0]))
                })
                .Where(x =>
                    x.tz.Id.Contains(option.Value ?? string.Empty, StringComparison.InvariantCultureIgnoreCase)
                    || x.tz.DisplayName.Contains(option.Value ?? string.Empty, StringComparison.InvariantCultureIgnoreCase)
                    || x.StdAbbrev.Equals(option.Value ?? string.Empty, StringComparison.InvariantCultureIgnoreCase)
                    || x.DstAbbrev.Equals(option.Value ?? string.Empty, StringComparison.InvariantCultureIgnoreCase)
                )
                .OrderBy(x => !(x.StdAbbrev.Equals(option.Value, StringComparison.InvariantCultureIgnoreCase)
                                || x.DstAbbrev.Equals(option.Value, StringComparison.InvariantCultureIgnoreCase)))
                .ThenBy(x => x.tz.Id, StringComparer.InvariantCultureIgnoreCase)
                .Take(25)
                .Select(x => new ApplicationCommandOptionChoiceProperties(x.tz.DisplayName, x.tz.Id));
            
            return timezones;
        }
    }
}