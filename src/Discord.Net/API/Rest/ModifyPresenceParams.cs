namespace Discord.API.Rest
{
    public class ModifyPresenceParams
    {
        internal Optional<UserStatus> _status;
        public UserStatus Status { set { _status = value; } }

        internal Optional<Discord.Game> _game;
        public Discord.Game Game { set { _game = value; } }
    }
}
