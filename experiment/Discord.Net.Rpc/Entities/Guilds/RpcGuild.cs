using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Model = Discord.API.Rpc.Guild;

namespace Discord.Rpc
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RpcGuild : RpcEntity<ulong>
    {
        public string Name { get; private set; }
        public string IconUrl { get; private set; }
        public IReadOnlyCollection<RpcGuildUser> Users { get; private set; }

        internal RpcGuild(DiscordRpcClient discord, ulong id)
            : base(discord, id)
        {
        }
        internal static RpcGuild Create(DiscordRpcClient discord, Model model)
        {
            var entity = new RpcGuild(discord, model.Id);
            entity.Update(model);
            return entity;
        }
        internal void Update(Model model)
        {
            Name = model.Name;
            IconUrl = model.IconUrl;
            Users = model.Members.Select(x => RpcGuildUser.Create(Discord, x)).ToImmutableArray();
        }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id})";
    }
}
