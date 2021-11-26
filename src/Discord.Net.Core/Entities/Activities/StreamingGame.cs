using System.Diagnostics;

namespace Discord
{
    /// <summary>
    ///     A user's activity for streaming on services such as Twitch.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class StreamingGame : Game
    {
        /// <summary>
        ///     Gets the URL of the stream.
        /// </summary>
        public string Url { get; internal set; }

        /// <summary>
        ///     Creates a new <see cref="StreamingGame" /> based on the <paramref name="name"/> on the stream URL.
        /// </summary>
        /// <param name="name">The name of the stream.</param>
        /// <param name="url">The URL of the stream.</param>
        public StreamingGame(string name, string url)
        {
            Name = name;
            Url = url;
            Type = ActivityType.Streaming;
        }

        /// <summary>
        ///     Gets the name of the stream.
        /// </summary>
        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Url})";
    }
}
