using System.Diagnostics;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RichGame : Game
    {
        internal RichGame() { }

        public string Details { get; internal set; }
        public string State { get; internal set; }
        public ulong ApplicationId { get; internal set; }
        public GameAsset SmallAsset { get; internal set; }
        public GameAsset LargeAsset { get; internal set; }
        public GameParty Party { get; internal set; }
        public GameSecrets Secrets { get; internal set; }
        public GameTimestamps Timestamps { get; internal set; }
        
        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} (Rich)";
    }
}
