namespace Discord
{
    public class ChannelUserEventArgs 
    {
        public Channel Channel { get; }
        public User User { get; }

        public ChannelUserEventArgs(Channel channel, User user)
        {
            Channel = channel;
            User = user;
        }
    }
}
