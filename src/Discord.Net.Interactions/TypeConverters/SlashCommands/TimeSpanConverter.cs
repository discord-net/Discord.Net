using Discord.WebSocket;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    internal sealed class TimeSpanConverter : TypeConverter<TimeSpan>
    {
        public override ApplicationCommandOptionType GetDiscordType() => ApplicationCommandOptionType.String;
        public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IApplicationCommandInteractionDataOption option, IServiceProvider services)
        {
            return (TimeSpan.TryParseExact((option.Value as string).ToLowerInvariant(), Formats, CultureInfo.InvariantCulture, out var timeSpan))
                ? Task.FromResult(TypeConverterResult.FromSuccess(timeSpan))
                : Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, "Failed to parse TimeSpan"));
        }

        private static readonly string[] Formats = {
            "%d'd'%h'h'%m'm'%s's'", //4d3h2m1s
            "%d'd'%h'h'%m'm'",      //4d3h2m
            "%d'd'%h'h'%s's'",      //4d3h  1s
            "%d'd'%h'h'",           //4d3h
            "%d'd'%m'm'%s's'",      //4d  2m1s
            "%d'd'%m'm'",           //4d  2m
            "%d'd'%s's'",           //4d    1s
            "%d'd'",                //4d
            "%h'h'%m'm'%s's'",      //  3h2m1s
            "%h'h'%m'm'",           //  3h2m
            "%h'h'%s's'",           //  3h  1s
            "%h'h'",                //  3h
            "%m'm'%s's'",           //    2m1s
            "%m'm'",                //    2m
            "%s's'",                //      1s
        };
    }
}
