using System;

namespace Discord
{
    /// <summary>
    ///     Represents a class used to make timestamps in messages. see <see href="https://discord.com/developers/docs/reference#message-formatting-timestamp-styles"/>.
    /// </summary>
    public readonly struct TimestampTag
    {
        /// <summary>
        ///     Gets the time for this timestamp tag.
        /// </summary>
        public DateTimeOffset Time { get; }

        /// <summary>
        ///     Gets the style of this tag. <see langword="null"/> if none was provided.
        /// </summary>
        public TimestampTagStyles? Style { get; }

        /// <summary>
        ///     Creates a new <see cref="TimestampTag"/> from the provided time.
        /// </summary>
        /// <param name="time">The time for this timestamp tag.</param>
        /// <param name="style">The style for this timestamp tag.</param>
        public TimestampTag(DateTimeOffset time, TimestampTagStyles? style = null)
        {
            Time = time;
            Style = style;
        }

        /// <summary>
        ///     Converts the current timestamp tag to the string representation supported by discord.
        ///     <para>
        ///         If the <see cref="Time"/> is null then the default 0 will be used.
        ///     </para>
        /// </summary>
        /// <remarks>
        ///     Will use the provided <see cref="Style"/> if provided. If this value is null, it will default to <see cref="TimestampTagStyles.ShortDateTime"/>.
        /// </remarks>
        /// <returns>A string that is compatible in a discord message, ex: <code>&lt;t:1625944201:f&gt;</code></returns>
        public override string ToString()
            => ToString(Style ?? TimestampTagStyles.ShortDateTime);

        /// <summary>
        ///     Converts the current timestamp tag to the string representation supported by discord.
        ///     <para>
        ///         If the <see cref="Time"/> is null then the default 0 will be used.
        ///     </para>
        /// </summary>
        /// <param name="style">The formatting style for this tag.</param>
        /// <returns>A string that is compatible in a discord message, ex: <code>&lt;t:1625944201:f&gt;</code></returns>
        public string ToString(TimestampTagStyles style)
            => $"<t:{Time.ToUnixTimeSeconds()}:{(char)style}>";

        /// <summary>
        ///     Creates a new timestamp tag with the specified <see cref="DateTime"/> object.
        /// </summary>
        /// <param name="time">The time of this timestamp tag.</param>
        /// <param name="style">The style for this timestamp tag.</param>
        /// <returns>The newly create timestamp tag.</returns>
        public static TimestampTag FromDateTime(DateTime time, TimestampTagStyles? style = null)
            => new(time, style);

        /// <summary>
        ///     Creates a new timestamp tag with the specified <see cref="DateTimeOffset"/> object.
        /// </summary>
        /// <param name="time">The time of this timestamp tag.</param>
        /// <param name="style">The style for this timestamp tag.</param>
        /// <returns>The newly create timestamp tag.</returns>
        public static TimestampTag FromDateTimeOffset(DateTimeOffset time, TimestampTagStyles? style = null)
            => new(time, style);

        /// <summary>
        ///     Immediately formats the provided time and style into a timestamp string.
        /// </summary>
        /// <param name="time">The time of this timestamp tag.</param>
        /// <param name="style">The style for this timestamp tag.</param>
        /// <returns>The newly create timestamp string.</returns>
        public static string FormatFromDateTime(DateTime time, TimestampTagStyles style)
            => FormatFromDateTimeOffset(time, style);

        /// <summary>
        ///     Immediately formats the provided time and style into a timestamp string.
        /// </summary>
        /// <param name="time">The time of this timestamp tag.</param>
        /// <param name="style">The style for this timestamp tag.</param>
        /// <returns>The newly create timestamp string.</returns>
        public static string FormatFromDateTimeOffset(DateTimeOffset time, TimestampTagStyles style)
            => $"<t:{time.ToUnixTimeSeconds()}:{(char)style}>";
    }
}
