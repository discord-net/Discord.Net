using System.Diagnostics;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct Activity
    {
        public string Name { get; }
        public string StreamUrl { get; }
        public ActivityType Type { get; }

        public Activity(string name, string streamUrl, ActivityType type)
        {
            Name = name;
            StreamUrl = streamUrl;
            Type = type;
        }
        private Activity(string name)
            : this(name, null, ActivityType.Playing) { }

        public override string ToString() => Name;
        private string DebuggerDisplay => StreamUrl != null ? $"{Name} ({StreamUrl})" : Name;
    }
}
