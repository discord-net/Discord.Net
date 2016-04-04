namespace Discord
{
    public class PublicUser : User
    {
        public override DiscordClient Discord { get; }

        internal PublicUser(ulong id, DiscordClient discord)
            : base(id)
        {
            Discord = discord;
        }
    }
}
