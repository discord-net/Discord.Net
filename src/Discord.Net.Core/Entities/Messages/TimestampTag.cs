using System;

namespace Discord
{
    /// <summary>
    ///     Represents a class used to make timestamps in messages. see <see href="https://discord.com/developers/docs/reference#message-formatting-timestamp-styles"/>.
    /// </summary>
    public class TimestampTag
    {
        /// <summary>
        ///     Gets or sets the style of the timestamp tag.
        /// </summary>
        public TimestampTagStyles Style { get; set; } = TimestampTagStyles.ShortDateTime;

        /// <summary>
        ///     Gets or sets the time for this timestamp tag.
        /// </summary>
        public DateTimeOffset Time { get; set; }

        /// <summary>
        ///     Converts the current timestamp tag to the string representation supported by discord.
        ///     <para>
        ///         If the <see cref="Time"/> is null then the default 0 will be used.
        ///     </para>
        /// </summary>
        /// <returns>A string that is compatible in a discord message, ex: <code>&lt;t:1625944201:f&gt;</code></returns>
        public override string ToString()
        {
            return $"<t:{Time.ToUnixTimeSeconds()}:{(char)Style}>";
        }

        /// <summary>
        ///     Creates a new timestamp tag with the specified <see cref="DateTime"/> object.
        /// </summary>
        /// <param name="time">The time of this timestamp tag.</param>
        /// <param name="style">The style for this timestamp tag.</param>
        /// <returns>The newly create timestamp tag.</returns>
        public static TimestampTag FromDateTime(DateTime time, TimestampTagStyles style = TimestampTagStyles.ShortDateTime)
        {
            return new TimestampTag
            {
                Style = style,
                Time = time
            };
        }

        /// <summary>
        ///     Creates a new timestamp tag with the specified <see cref="DateTimeOffset"/> object.
        /// </summary>
        /// <param name="time">The time of this timestamp tag.</param>
        /// <param name="style">The style for this timestamp tag.</param>
        /// <returns>The newly create timestamp tag.</returns>
        public static TimestampTag FromDateTimeOffset(DateTimeOffset time, TimestampTagStyles style = TimestampTagStyles.ShortDateTime)
        {
            return new TimestampTag
            {
                Style = style,
                Time  = time
            };
        }
    }
}