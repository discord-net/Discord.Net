using System.Diagnostics;
using Model = Discord.API.Presence;

namespace Discord.WebSocket
{
    //TODO: C#7 Candidate for record type
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct SocketPresence : IPresence
    {
        public Game? Game { get; }
        public UserStatus Status { get; }

        internal SocketPresence(Game? game, UserStatus status)
        {
            Game = game;
            Status = status;
        }
        internal static SocketPresence Create(Model model)
        {
            return new SocketPresence(model.Game != null ? Discord.Game.Create(model.Game) : (Game?)null, model.Status);
        }

        public override string ToString() => Status.ToString();
        internal string DebuggerDisplay => $"{Status}{(Game != null ? $", {Game.Value.Name} ({Game.Value.StreamType})" : "")}";

        internal SocketPresence Clone() => this;
    }
}
