using Discord.Rpc;

namespace Discord.Commands
{
    public class RpcCommandContext : ICommandContext
    {
        public DiscordRpcClient Client { get; }
        public IMessageChannel Channel { get; }
        public RpcUser User { get; }
        public RpcUserMessage Message { get; }

        public bool IsPrivate => Channel is IPrivateChannel;

        public RpcCommandContext(DiscordRpcClient client, RpcUserMessage msg)
        {
            Client = client;
            Channel = msg.Channel;
            User = msg.Author;
            Message = msg;
        }

        //ICommandContext
        IDiscordClient ICommandContext.Client => Client;
        IGuild ICommandContext.Guild => null;
        IMessageChannel ICommandContext.Channel => Channel;
        IUser ICommandContext.User => User;
        IUserMessage ICommandContext.Message => Message;
    }
}
