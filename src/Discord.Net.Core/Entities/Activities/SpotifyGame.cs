using System.Collections.Generic;
using System.Diagnostics;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SpotifyGame : Game
    {
        internal SpotifyGame() { }

        public string TrackTitle { get; internal set; }
        public string TrackAlbum { get; internal set; }
        public string SyncId { get; internal set; }
        public string SessionId { get; internal set; }
        public string[] Artists { get; internal set; }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} (Spotify)";
    }
}
