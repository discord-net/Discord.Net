namespace Discord
{
    internal struct Presence : IPresence
    {
        public UserStatus Status { get; }
        public Game? Game { get; }
        
        public Presence(UserStatus status, Game? game)
        {
            Status = status;
            Game = game;
        }
    }
}
