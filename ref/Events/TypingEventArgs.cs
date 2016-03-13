namespace Discord
{
    public class TypingEventArgs 
    {
        public ITextChannel Channel { get; }
        public IUser User { get; }

        public TypingEventArgs(ITextChannel channel, IUser user)
        {
            Channel = channel;
            User = user;
        }
    }
}
