using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Discord
{
    /// <summary>
    ///     A user's activity for listening to a song on Spotify.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SpotifyGame : Game
    {
        /// <summary>
        ///     Gets the song's artist(s).
        /// </summary>
        public IEnumerable<string> Artists { get; internal set; }
        /// <summary>
        ///     Gets the Spotify album art of the song.
        /// </summary>
        public string AlbumArt { get; internal set; }
        /// <summary>
        ///     Gets the Spotify album title of the song.
        /// </summary>
        public string AlbumTitle { get; internal set; }
        /// <summary>
        ///     Gets the track title of the song.
        /// </summary>
        public string TrackTitle { get; internal set; }
        /// <summary>
        ///     Gets the synchronization ID of the song.
        /// </summary>
        public string SyncId { get; internal set; }
        /// <summary>
        ///     Gets the session ID of the song.
        /// </summary>
        public string SessionId { get; internal set; }
        /// <summary>
        ///     Gets the duration of the song.
        /// </summary>
        public TimeSpan? Duration { get; internal set; }

        internal SpotifyGame() { }

        /// <summary> Gets the name of the song. </summary>
        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} (Spotify)";
    }
}
