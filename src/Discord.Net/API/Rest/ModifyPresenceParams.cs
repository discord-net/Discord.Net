namespace Discord.API.Rest
{
    public class ModifyPresenceParams
    {
        public Optional<UserStatus> Status { get; set; }
        public Optional<Discord.Game> Game { get; set; }
    }
}
