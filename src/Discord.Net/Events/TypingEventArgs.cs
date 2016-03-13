namespace Discord
{
    public class TypingEventArgs 
    {
        public ITextChannel Channel { get; }
        public User User { get; }

        public TypingEventArgs(ITextChannel channel, User user)
        {
            Channel = channel;
            User = user;
        }
    }
}
