#pragma warning disable CS1591
namespace Discord.API.Rest
{
    public class ModifyPresenceParams
    {
        internal Optional<UserStatus> _status { get; set; }
        public UserStatus Status { set { _status = value; } }

        internal Optional<Discord.Game> _game { get; set; }
        public Discord.Game Game { set { _game = value; } }
    }
}
