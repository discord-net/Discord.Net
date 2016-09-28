using Model = Discord.API.Presence;

namespace Discord.WebSocket
{
    //TODO: C#7 Candidate for record type
    internal struct SocketPresence : IPresence
    {
        public Game? Game { get; }
        public UserStatus Status { get; }

        internal SocketPresence(Game? game, UserStatus status)
        {
            Game = game;
            Status = status;
        }
        internal SocketPresence Create(Model model)
        {
            return new SocketPresence(model.Game != null ? Discord.Game.Create(model.Game) : (Game?)null, model.Status);
        }

        public SocketPresence Clone() => this;
    }
}
