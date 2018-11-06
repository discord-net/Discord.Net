using System.Diagnostics;

namespace Discord
{
    /// <summary>
    ///     A user's Rich Presence status.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RichGame : Game
    {
        internal RichGame() { }

        /// <summary>
        ///     Gets what the player is currently doing.
        /// </summary>
        public string Details { get; internal set; }
        /// <summary>
        ///     Gets the user's current party status.
        /// </summary>
        public string State { get; internal set; }
        /// <summary>
        ///     Gets the application ID for the game.
        /// </summary>
        public ulong ApplicationId { get; internal set; }
        /// <summary>
        ///     Gets the small image for the presence and their hover texts.
        /// </summary>
        public GameAsset SmallAsset { get; internal set; }
        /// <summary>
        ///     Gets the large image for the presence and their hover texts.
        /// </summary>
        public GameAsset LargeAsset { get; internal set; }
        /// <summary>
        ///     Gets the information for the current party of the player.
        /// </summary>
        public GameParty Party { get; internal set; }
        /// <summary>
        ///     Gets the secrets for Rich Presence joining and spectating.
        /// </summary>
        public GameSecrets Secrets { get; internal set; }
        /// <summary>
        ///     Gets the timestamps for start and/or end of the game.
        /// </summary>
        public GameTimestamps Timestamps { get; internal set; }

        /// <summary>
        ///     Returns the name of the Rich Presence.
        /// </summary>
        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} (Rich)";
    }
}
