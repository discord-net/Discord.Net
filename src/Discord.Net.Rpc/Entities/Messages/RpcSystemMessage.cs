using Discord.Rest;
using System.Diagnostics;
using Model = Discord.API.Rpc.Message;

namespace Discord.Rpc
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RpcSystemMessage : RpcMessage, ISystemMessage
    {
        public MessageType Type { get; private set; }

        internal RpcSystemMessage(DiscordRpcClient discord, ulong id, RestVirtualMessageChannel channel, RpcUser author)
            : base(discord, id, channel, author, MessageSource.System)
        {
        }
        internal new static RpcSystemMessage Create(DiscordRpcClient discord, ulong channelId, Model model)
        {
            var entity = new RpcSystemMessage(discord, model.Id,
                RestVirtualMessageChannel.Create(discord, channelId),
                RpcUser.Create(discord, model.Author.Value, model.WebhookId.ToNullable()));
            entity.Update(model);
            return entity;
        }
        internal override void Update(Model model)
        {
            base.Update(model);

            Type = model.Type;
        }

        private string DebuggerDisplay => $"{Author}: {Content} ({Id}, {Type})";
    }
}
