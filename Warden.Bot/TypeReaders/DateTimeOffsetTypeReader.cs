using System.Globalization;
using NetCord.Services;

namespace Warden.Bot.TypeReaders;

public class DateTimeOffsetTypeReader<TContext> : SlashCommandTypeReader<TContext>
    where TContext : IApplicationCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.String;

    public override ValueTask<TypeReaderResult> ReadAsync(string value, TContext context,
        SlashCommandParameter<TContext> parameter, ApplicationCommandServiceConfiguration<TContext> configuration,
        IServiceProvider? serviceProvider) =>
        new(DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTimeOffset result)
            ? TypeReaderResult.Success(result)
            : TypeReaderResult.ParseFail(parameter.Name));
}