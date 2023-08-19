using System;
using System.Diagnostics;

namespace Discord
{
    /// <summary>
    ///     A user's activity for their custom status.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class CustomStatusGame : Game
    {
        internal CustomStatusGame() { }

        /// <summary>
        ///     Creates a new custom status activity.
        /// </summary>
        /// <remarks>
        ///     Bots can't set custom status emoji.
        /// </remarks>
        /// <param name="state">The string displayed as bot's custom status.</param>
        public CustomStatusGame(string state)
        {
            Name = "Custom Status";
            State = state;
            Type = ActivityType.CustomStatus;
        }

        /// <summary>
        ///     Gets the emote, if it is set.
        /// </summary>
        /// <returns>
        ///     An <see cref="IEmote"/> containing the <see cref="Emoji"/> or <see cref="GuildEmote"/> set by the user.
        /// </returns>
        public IEmote Emote { get; internal set; }

        /// <summary>
        ///     Gets the timestamp of when this status was created.
        /// </summary>
        /// <returns>
        ///     A <see cref="DateTimeOffset"/> containing the time when this status was created.
        /// </returns>
        public DateTimeOffset CreatedAt { get; internal set; }

        /// <summary>
        ///     Gets the state of the status.
        /// </summary>
        public string State { get; internal set; }

        public override string ToString()
            => $"{Emote} {State}";

        private string DebuggerDisplay => $"{Name}";
    }
}
