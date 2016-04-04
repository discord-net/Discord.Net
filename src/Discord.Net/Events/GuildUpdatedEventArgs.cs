namespace Discord
{
    public class GuildUpdatedEventArgs : GuildEventArgs
    {
        public Guild Before { get; }
        public Guild After => Guild;

        public GuildUpdatedEventArgs(Guild before, Guild after)
            : base(after)
        {
            Before = before;
        }
    }
}
