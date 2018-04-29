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
        public IReadOnlyCollection<string> Artists { get; internal set; }
        /// <summary>
        ///     Gets the Spotify album title of the song.
        /// </summary>
        public string AlbumTitle { get; internal set; }
        /// <summary>
        ///     Gets the track title of the song.
        /// </summary>
        public string TrackTitle { get; internal set; }
        /// <summary>
        ///     Gets the duration of the song.
        /// </summary>
        public TimeSpan? Duration { get; internal set; }

        /// <summary>
        ///     Gets the track ID of the song.
        /// </summary>
        public string TrackId { get; internal set; }
        /// <summary>
        ///     Gets the session ID of the song.
        /// </summary>
        public string SessionId { get; internal set; }

        /// <summary>
        ///     Gets the URL of the album art.
        /// </summary>
        public string AlbumArtUrl { get; internal set; }
        /// <summary>
        ///     Gets the direct Spotify URL of the track.
        /// </summary>
        public string TrackUrl { get; internal set; }

        internal SpotifyGame() { }

        /// <summary>
        ///     Gets the full information of the song.
        /// </summary>
        public override string ToString() => $"{string.Join(", ", Artists)} - {TrackTitle} ({Duration})";
        private string DebuggerDisplay => $"{Name} (Spotify)";
    }
}
