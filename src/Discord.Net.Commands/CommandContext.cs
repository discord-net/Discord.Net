namespace Discord.Commands
{
    public struct CommandContext
    {
        public IDiscordClient Client { get; }
        public IGuild Guild { get; }
        public IMessageChannel Channel { get; }
        public IUser User { get; }
        public IUserMessage Message { get; }

        public bool IsPrivate => Channel is IPrivateChannel;

        public CommandContext(IDiscordClient client, IGuild guild, IMessageChannel channel, IUser user, IUserMessage msg)
        {
            Client = client;
            Guild = guild;
            Channel = channel;
            User = user;
            Message = msg;
        }
        public CommandContext(IDiscordClient client, IUserMessage msg)
        {
            Client = client;
            Guild = (msg.Channel as IGuildChannel).Guild;
            Channel = msg.Channel;
            User = msg.Author;
            Message = msg;
        }
    }
}
