using System.Diagnostics;
using Model = Discord.API.Presence;

namespace Discord.WebSocket
{
    //TODO: C#7 Candidate for record type
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct SocketPresence : IPresence
    {
        public UserStatus Status { get; }
        public Game? Game { get; }

        internal SocketPresence(UserStatus status, Game? game)
        {
            Status = status;
            Game = game;
        }
        internal static SocketPresence Create(Model model)
        {
            return new SocketPresence(model.Status, model.Game != null ? model.Game.ToEntity() : (Game?)null);
        }

        public override string ToString() => Status.ToString();
        private string DebuggerDisplay => $"{Status}{(Game != null ? $", {Game.Value.Name} ({Game.Value.StreamType})" : "")}";

        internal SocketPresence Clone() => this;
    }
}
