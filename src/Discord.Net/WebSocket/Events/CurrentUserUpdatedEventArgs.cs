namespace Discord.WebSocket
{
    public class CurrentUserUpdatedEventArgs : CurrentUserEventArgs
    {
        public SelfUser Before { get; }
        public SelfUser After => CurrentUser;

        public CurrentUserUpdatedEventArgs(SelfUser before, SelfUser after)
            : base(after)
        {
            Before = before;
        }
    }
}
