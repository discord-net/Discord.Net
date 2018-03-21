namespace Discord.Commands
{
    public class CommandContext : ICommandContext
    {
        /// <inheritdoc/>
        public IDiscordClient Client { get; }
        /// <inheritdoc/>
        public IGuild Guild { get; }
        /// <inheritdoc/>
        public IMessageChannel Channel { get; }
        /// <inheritdoc/>
        public IUser User { get; }
        /// <inheritdoc/>
        public IUserMessage Message { get; }

        /// <summary> Indicates whether the channel that the command is executed in is a private channel. </summary>
        public bool IsPrivate => Channel is IPrivateChannel;
        
        public CommandContext(IDiscordClient client, IUserMessage msg)
        {
            Client = client;
            Guild = (msg.Channel as IGuildChannel)?.Guild;
            Channel = msg.Channel;
            User = msg.Author;
            Message = msg;
        }
    }
}
