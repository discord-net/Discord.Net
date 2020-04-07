using System;

namespace Discord
{
    /// <summary>
    /// Utilities for reading and writing Discord snowflakes.
    /// <seealso href="https://discordapp.com/developers/docs/reference#snowflakes"/>
    /// </summary>
    public static class Snowflake
    {
        /// <summary>
        /// The offset, in milliseconds, from the Unix epoch which represents
        /// the Discord Epoch.
        /// </summary>
        public const ulong DiscordEpochOffset = 1420070400000UL;

        /// <summary>
        /// Calculates the time a given snowflake was created.
        /// </summary>
        /// <param name="snowflake">
        /// The snowflake to calculate the creation time of.
        /// </param>
        /// <returns>
        /// A <see cref="DateTimeOffset"/> representing the creation time, in
        /// UTC, of the snowflake.
        /// </returns>
        /// <example>
        /// This sample demonstrates how to identify when a Discord user was
        /// created.
        /// <code>
        /// IUser user = await GetUserAsync();
        /// var snowflake = user.Id;
        /// var created = Snowflake.GetCreatedTime(snowflake);
        /// Console.WriteLine($"The user {user.Name} was created at {created}");
        /// </code>
        /// </example>
        public static DateTimeOffset GetCreatedTime(ulong snowflake)
            => DateTimeOffset.FromUnixTimeMilliseconds(
                (long)((snowflake >> 22) + DiscordEpochOffset));

        /// <summary>
        /// Calculates the smallest possible snowflake for a given creation
        /// time.
        /// </summary>
        /// <param name="time">
        /// The time to generate a snowflake for.
        /// </param>
        /// <returns>
        /// A snowflake representing the smallest possible snowflake for the
        /// given creation time.
        /// </returns>
        /// <example>
        /// This sample demonstrates how to check if a user was created before
        /// a certain date.
        /// <code>
        /// IUser user = await GetUserAsync();
        /// var desiredTime = DateTimeOffset.UtcNow.AddDays(-7);
        /// var minimumSnowflake = Snowflake.GetSnowflake(desiredTime);
        ///
        /// if (user.Id &lt;= minimumSnowflake)
        ///     Console.WriteLine($"The user {user.Name} was created at least 7 days ago");
        /// else
        ///     Console.WriteLine($"The user {user.Name} was created less than 7 days ago");
        /// </code>
        /// </example>
        public static ulong GetSnowflake(DateTimeOffset time)
            => ((ulong)time.ToUnixTimeMilliseconds()
                - DiscordEpochOffset) << 22;
    }
}
