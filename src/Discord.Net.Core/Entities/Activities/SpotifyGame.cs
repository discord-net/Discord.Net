using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SpotifyGame : Game
    {
        public IReadOnlyCollection<string> Artists { get; internal set; }
        public string AlbumTitle { get; internal set; }
        public string TrackTitle { get; internal set; }
        public TimeSpan? Duration { get; internal set; }

        public string TrackId { get; internal set; }
        public string SessionId { get; internal set; }

        public string AlbumArtUrl { get; internal set; }
        public string TrackUrl { get; internal set; }

        internal SpotifyGame() { }

        public override string ToString() => $"{string.Join(", ", Artists)} - {TrackTitle} ({Duration})";
        private string DebuggerDisplay => $"{Name} (Spotify)";
    }
}
