using System.Diagnostics;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class Game : IActivity
    {
        public string Name { get; internal set; }

        public Game() { }
        public Game(string name)
        {
            Name = name;
        }
        
        public override string ToString() => Name;
        private string DebuggerDisplay => Name;
    }
}
