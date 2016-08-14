namespace Discord.WebSocket
{
    //TODO: C#7 Candidate for record type
    internal struct Presence : IPresence
    {
        public Game Game { get; }
        public UserStatus Status { get; }

        public Presence(Game game, UserStatus status)
        {
            Game = game;
            Status = status;
        }

        public Presence Clone() => this;
    }
}
