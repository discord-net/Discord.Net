using System;
using System.Diagnostics;

namespace Discord
{
    // TODO 2.x: make this a class?
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct Game
    {
        // All
        public string Name { get; }
        public StreamType StreamType { get; }
        // Streaming
        public string StreamUrl { get; }
        // Rich
        public string Details { get; }
        public string State { get; }
        public ulong? ApplicationId { get; }
        public GameAssets? Assets { get; }
        public GameParty? Party { get; }
        public GameSecrets? Secrets { get; }
        public GameTimestamps? Timestamps { get; }

        public Game(string name, string streamUrl = null, StreamType type = StreamType.NotStreaming)
            : this(name, streamUrl, type, null, null, null, null, null, null, null) { }
        public Game(string name,
            string streamUrl,
            StreamType type,
            string details,
            string state,
            ulong? applicationId,
            GameAssets? assets,
            GameParty? party,
            GameSecrets? secrets,
            GameTimestamps? timestamps)
        {
            Name = name;
            StreamUrl = streamUrl;
            StreamType = type;
            Details = details;
            State = state;
            ApplicationId = applicationId;
            Assets = assets;
            Party = party;
            Secrets = secrets;
            Timestamps = timestamps;
        }

        public override string ToString() => Name;
        private string DebuggerDisplay => StreamUrl != null ? $"{Name} ({StreamUrl})" : Name;
    }
}
