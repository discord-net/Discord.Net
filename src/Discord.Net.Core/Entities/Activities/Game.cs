using System.Diagnostics;

namespace Discord
{
    [DebuggerDisplay(@"{" + nameof(DebuggerDisplay) + @",nq}")]
    public class Game : IActivity
    {
        internal Game()
        {
        }

        public Game(string name, ActivityType type = ActivityType.Playing)
        {
            Name = name;
            Type = type;
        }

        private string DebuggerDisplay => Name;
        public string Name { get; internal set; }
        public ActivityType Type { get; internal set; }

        public override string ToString() => Name;
    }
}
