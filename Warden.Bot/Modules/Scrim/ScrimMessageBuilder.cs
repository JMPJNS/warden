using NetCord.Rest;

namespace Warden.Bot.Modules.Scrim;

public class ScrimMessageBuilder
{
    public static MessageProperties Build(Data.Models.Scrim scrim, bool cancelled = false)
    {
        var embed = new EmbedProperties()
            .WithTitle(cancelled ? "~~Scrim Cancelled~~" : $"Scrim on <t:{scrim.Time.ToUnixTimeSeconds()}>")
            .WithFields([
                new EmbedFieldProperties()
                    .WithInline()
                    .WithName("Sapphire Flame")
                    .WithValue(scrim.Team1.Name),
                new EmbedFieldProperties()
                    .WithInline()
                    .WithName("Amber Hand")
                    .WithValue(scrim.Team2?.Name ?? "❌")
            ]);

        if (scrim.Ringers.Any())
        {
            var ringerMentions = string.Join(", ", scrim.Ringers.Select(id => $"<@{id}>"));

            embed.Fields = embed.Fields?.Append(new EmbedFieldProperties()
                .WithName("Ringers")
                .WithValue(ringerMentions));
        }
        
        return new MessageProperties()
            .WithEmbeds([
                embed
            ])
            .WithComponents([
                new ActionRowProperties()
                    .AddButtons(new ButtonProperties($"join scrim:{scrim.Id}", "Sign up", ButtonStyle.Primary),
                        new ButtonProperties($"scrim looking ringer:{scrim.Id}", "Ask for Ringer", ButtonStyle.Secondary),
                        new ButtonProperties($"scrim cancel:{scrim.Id}", "Cancel", ButtonStyle.Danger))
            ])
            .WithAllowedMentions(AllowedMentionsProperties.None);
    }

    public static MessageProperties BuildRingerMessage(Data.Models.Scrim scrim)
    {
        var embed = new EmbedProperties()
            .WithTitle($"Scrim on <t:{scrim.Time.ToUnixTimeSeconds()}>")
            .WithDescription($"Looking for <@&1364234481115988048>!");
        
        return new MessageProperties()
            .WithEmbeds([
                embed
            ])
            .WithComponents([
                new ActionRowProperties()
                    .AddButtons(new ButtonProperties($"scrim ringer:{scrim.Id}", "Sign up", ButtonStyle.Primary))
            ])
            .WithAllowedMentions(new()
            {
                AllowedRoles = [1364234481115988048]
            });;
    }
}