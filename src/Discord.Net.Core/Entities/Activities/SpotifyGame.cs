using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SpotifyGame : Game
    {
        public string[] Artists { get; internal set; }
        public string AlbumArt { get; internal set; }
        public string AlbumTitle { get; internal set; }
        public string TrackTitle { get; internal set; }
        public string SyncId { get; internal set; }
        public string SessionId { get; internal set; }
        public TimeSpan? Duration { get; internal set; }

        internal SpotifyGame() { }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} (Spotify)";
    }
}
