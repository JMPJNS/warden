using NetCord.Rest;

namespace Warden.Bot.Modules.Scrim;

public class ScrimMessageBuilder
{
    public static MessageProperties Build(Data.Models.Scrim scrim, bool cancelled = false)
    {
        var embed = new EmbedProperties()
            .WithTitle(cancelled ? "~~Scrim Cancelled~~" : $"Scrim at <t:{scrim.Time.ToUnixTimeSeconds()}>")
            .WithTimestamp(scrim.Time.DateTime)
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
                        new ButtonProperties($"scrim ringer:{scrim.Id}", "Sign up as Ringer", ButtonStyle.Secondary),
                        new ButtonProperties($"scrim cancel:{scrim.Id}", "Cancel", ButtonStyle.Danger))
            ])
            .WithAllowedMentions(AllowedMentionsProperties.None);
    }
}