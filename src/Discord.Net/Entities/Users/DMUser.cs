namespace Discord
{
    public class DMUser : User
    {
        public DMChannel Channel { get; }

        public override DiscordClient Discord => Channel.Discord;

        internal DMUser(ulong id, DMChannel channel)
            : base(id)
        {
            Channel = channel;
        }
    }
}
