using Discord.Rest;

namespace Discord.Rpc
{
    internal class RpcMessage : Message
    {
        public override DiscordRestClient Discord { get; }

        public RpcMessage(DiscordRpcClient discord, API.Message model)
            : base(null, model.Author.IsSpecified ? new User(model.Author.Value) : null, model)
        {
            Discord = discord;
        }
    }
}
