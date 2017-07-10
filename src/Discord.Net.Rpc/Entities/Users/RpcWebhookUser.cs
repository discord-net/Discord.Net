using System.Diagnostics;
using Model = Discord.API.User;

namespace Discord.Rpc
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RpcWebhookUser : RpcUser
    {
        public ulong WebhookId { get; }
        
        public override bool IsWebhook => true;

        internal RpcWebhookUser(DiscordRpcClient discord, ulong id, ulong webhookId)
            : base(discord, id)
        {
            WebhookId = webhookId;
        }
        internal static RpcWebhookUser Create(DiscordRpcClient discord, Model model, ulong webhookId)
        {
            var entity = new RpcWebhookUser(discord, model.Id, webhookId);
            entity.Update(model);
            return entity;
        }
    }
}
