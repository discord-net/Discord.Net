using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    internal class TimeSpanTypeReader : TypeReader<TimeSpan>
    {
        /// <summary>
        /// TimeSpan try parse formats.
        /// </summary>
        private static readonly string[] Formats =
        {
            "%d'd'%h'h'%m'm'%s's'", // 4d3h2m1s
            "%d'd'%h'h'%m'm'",      // 4d3h2m
            "%d'd'%h'h'%s's'",      // 4d3h  1s
            "%d'd'%h'h'",           // 4d3h
            "%d'd'%m'm'%s's'",      // 4d  2m1s
            "%d'd'%m'm'",           // 4d  2m
            "%d'd'%s's'",           // 4d    1s
            "%d'd'",                // 4d
            "%h'h'%m'm'%s's'",      //   3h2m1s
            "%h'h'%m'm'",           //   3h2m
            "%h'h'%s's'",           //   3h  1s
            "%h'h'",                //   3h
            "%m'm'%s's'",           //     2m1s
            "%m'm'",                //     2m
            "%s's'",                //       1s
        };

        /// <inheritdoc />
        public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, object input, IServiceProvider services)
        {
            var str = input as string;

            if (string.IsNullOrEmpty(str))
                throw new ArgumentException($"{nameof(input)} must not be null or empty.", nameof(input));

            var isNegative = str[0] == '-'; // Char for CultureInfo.InvariantCulture.NumberFormat.NegativeSign
            if (isNegative)
            {
                str = str.Substring(1);
            }

            if (TimeSpan.TryParseExact(str.ToLowerInvariant(), Formats, CultureInfo.InvariantCulture, out var timeSpan))
            {
                return isNegative
                    ? Task.FromResult(TypeConverterResult.FromSuccess(-timeSpan))
                    : Task.FromResult(TypeConverterResult.FromSuccess(timeSpan));
            }
            else
            {
                return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ParseFailed, "Failed to parse TimeSpan"));
            }
        }
    }
}
