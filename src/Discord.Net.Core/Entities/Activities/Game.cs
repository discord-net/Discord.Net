using System.Diagnostics;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class Game : IActivity
    {
        internal Game() { }

        public string Name { get; internal set; }
        public ActivityType Type { get; internal set; }

        public Game(string name, ActivityType type = ActivityType.Playing)
        {
            Name = name;
            Type = type;
        }
        
        public override string ToString() => Name;
        private string DebuggerDisplay => Name;
    }
}
