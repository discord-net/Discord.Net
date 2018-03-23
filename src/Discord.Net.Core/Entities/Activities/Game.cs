using System.Diagnostics;

namespace Discord
{
    /// <summary> A user's game activity. </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class Game : IActivity
    {
        /// <inheritdoc/>
        public string Name { get; internal set; }
        /// <inheritdoc/>
        public ActivityType Type { get; internal set; }

        internal Game() { }
        public Game(string name, ActivityType type = ActivityType.Playing)
        {
            Name = name;
            Type = type;
        }
        
        public override string ToString() => Name;
        private string DebuggerDisplay => Name;
    }
}
