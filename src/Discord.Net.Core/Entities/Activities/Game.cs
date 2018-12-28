using System.Diagnostics;

namespace Discord
{
    /// <summary>
    ///     A user's game status.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class Game : IActivity
    {
        /// <inheritdoc/>
        public string Name { get; internal set; }
        /// <inheritdoc/>
        public ActivityType Type { get; internal set; }

        internal Game() { }
        /// <summary>
        ///     Creates a <see cref="Game"/> with the provided name and <see cref="ActivityType"/>.
        /// </summary>
        /// <param name="name">The name of the game.</param>
        /// <param name="type">The type of activity.</param>
        public Game(string name, ActivityType type = ActivityType.Playing)
        {
            Name = name;
            Type = type;
        }
        
        /// <summary> Returns the name of the <see cref="Game"/>. </summary>
        public override string ToString() => Name;
        private string DebuggerDisplay => Name;
    }
}
