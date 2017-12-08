using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Rpc.Channel;

namespace Discord.Rpc
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RpcCategoryChannel : RpcGuildChannel, ICategoryChannel
    {
        public IReadOnlyCollection<RpcMessage> CachedMessages { get; private set; }

        public string Mention => MentionUtils.MentionChannel(Id);

        internal RpcCategoryChannel(DiscordRpcClient discord, ulong id, ulong guildId)
            : base(discord, id, guildId)
        {
        }
        internal new static RpcCategoryChannel Create(DiscordRpcClient discord, Model model)
        {
            var entity = new RpcCategoryChannel(discord, model.Id, model.GuildId.Value);
            entity.Update(model);
            return entity;
        }
        internal override void Update(Model model)
        {
            base.Update(model);
            CachedMessages = model.Messages.Select(x => RpcMessage.Create(Discord, Id, x)).ToImmutableArray();
        }
    }
}
