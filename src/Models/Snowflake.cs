using System;

namespace Discord.Net
{
    /// <summary>
    /// Represents a discord snowflake.
    /// </summary>
    public struct Snowflake
    {
        private const ulong DiscordEpoch = 1420070400000UL;

        /// <summary>
        /// Gets the raw value of this snowflake.
        /// </summary>
        /// <returns>A <see cref="ulong"/> with the rae value.</returns>
        public ulong RawValue { get; }

        /// <summary>
        /// Creates a <see cref="Snowflake"/> based on the <paramref name="value"/> provided.
        /// </summary>
        /// <param name="value">Raw value of the snowflake.</param>
        public Snowflake(ulong value)
        {
            RawValue = value;
        }

        /// <summary>
        /// Creates a <see cref="Snowflake"/> based on the <paramref name="dateTimeOffset"/> provided.
        /// </summary>
        /// <param name="dateTimeOffset">DateTimeOffset of this snowflake.</param>
        public Snowflake(DateTimeOffset dateTimeOffset)
        {
            RawValue = ((ulong)dateTimeOffset.ToUniversalTime().ToUnixTimeMilliseconds() - DiscordEpoch) << 22;
        }

        /// <summary>
        /// Creates a <see cref="Snowflake"/> based on the <paramref name="dateTime"/> provided.
        /// </summary>
        /// <param name="dateTime">DateTime of this snowflake.</param>
        public Snowflake(DateTime dateTime)
            : this(new DateTimeOffset(dateTime)) { }

        /// <summary>
        /// Converts this <see cref="Snowflake"/> to a <see cref="DateTimeOffset"/>.
        /// </summary>
        /// <returns>A <see cref="DateTimeOffset"/> of this snowflake.</returns>
        public DateTimeOffset ToDateTimeOffset() => DateTimeOffset.FromUnixTimeMilliseconds((long)((RawValue >> 22) + DiscordEpoch));

        /// <summary>
        /// Converts this <see cref="Snowflake"/> to a <see cref="DateTime"/>.
        /// </summary>
        /// <returns>A <see cref="DateTime"/> of this snowflake.</returns>
        public DateTimeOffset ToDateTime() => ToDateTimeOffset().UtcDateTime;

        /// <summary>
        /// Converts this <see cref="Snowflake"/> to a <see cref="ulong"/>.
        /// </summary>
        /// <param name="snowflake">Value that will be converted</param>
        /// <returns>A <see cref="ulong"/> with the raw value.</returns>
        public static implicit operator ulong(Snowflake snowflake) => snowflake.RawValue;

        /// <summary>
        /// Converts this <see cref="ulong"/> to a <see cref="Snowflake"/>.
        /// </summary>
        /// <param name="value">Value that will be converted</param>
        /// <returns>A <see cref="Snowflake"/> with <paramref name="value"/> as the raw value.</returns>
        public static implicit operator Snowflake(ulong value) => new Snowflake(value);

        /// <summary>
        /// Returns the raw value as <see cref="string"/>.
        /// </summary>
        /// <returns>A <see cref="string"/> that is the raw value.</returns>
        public override string ToString()
            => RawValue.ToString();
    }
}
