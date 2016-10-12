using System;

using Model = Discord.API.Rpc.Channel;

namespace Discord.Rpc
{
    public class RpcChannel : RpcEntity<ulong>
    {
        public string Name { get; private set; }

        public DateTimeOffset CreatedAt => DateTimeUtils.FromSnowflake(Id);

        internal RpcChannel(DiscordRpcClient discord, ulong id)
            : base(discord, id)
        {
        }
        internal static RpcChannel Create(DiscordRpcClient discord, Model model)
        {
            if (model.GuildId.IsSpecified)
                return RpcGuildChannel.Create(discord, model);
            else
                return CreatePrivate(discord, model);
        }
        internal static RpcChannel CreatePrivate(DiscordRpcClient discord, Model model)
        {
            switch (model.Type)
            {
                case ChannelType.DM:
                    return RpcDMChannel.Create(discord, model);
                case ChannelType.Group:
                    return RpcGroupChannel.Create(discord, model);
                default:
                    throw new InvalidOperationException($"Unexpected channel type: {model.Type}");
            }
        }
        internal virtual void Update(Model model)
        {
            if (model.Name.IsSpecified)
                Name = model.Name.Value;
        }
    }
}
