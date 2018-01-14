using System.Diagnostics;
using Model = Discord.API.Rpc.GuildSummary;

namespace Discord.Rpc
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RpcGuildSummary
    {
        public ulong Id { get; }
        public string Name { get; private set; }

        internal RpcGuildSummary(ulong id)
        {
            Id = id;
        }
        internal static RpcGuildSummary Create(Model model)
        {
            var entity = new RpcGuildSummary(model.Id);
            entity.Update(model);
            return entity;
        }
        internal void Update(Model model)
        {
            Name = model.Name;
        }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id})";
    }
}
