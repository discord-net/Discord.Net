using System.Diagnostics;
using Model = Discord.API.Rpc.GuildStatusEvent;

namespace Discord.Rpc
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RpcGuildStatus
    {
        public RpcGuildSummary Guild { get; }
        public int Online { get; private set; }

        internal RpcGuildStatus(ulong guildId)
        {
            Guild = new RpcGuildSummary(guildId);
        }
        internal static RpcGuildStatus Create(Model model)
        {
            var entity = new RpcGuildStatus(model.Guild.Id);
            entity.Update(model);
            return entity;
        }
        internal void Update(Model model)
        {
            Online = model.Online;
        }

        public override string ToString() => Guild.Name;
        private string DebuggerDisplay => $"{Guild.Name} ({Guild.Id}, {Online} Online)";
    }
}
