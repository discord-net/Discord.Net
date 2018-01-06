using System.Diagnostics;
using Model = Discord.API.Rpc.ChannelSummary;

namespace Discord.Rpc
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RpcChannelSummary
    {
        public ulong Id { get; }
        public string Name { get; private set; }
        public ChannelType Type { get; private set; }

        internal RpcChannelSummary(ulong id)
        {
            Id = id;
        }
        internal static RpcChannelSummary Create(Model model)
        {
            var entity = new RpcChannelSummary(model.Id);
            entity.Update(model);
            return entity;
        }
        internal void Update(Model model)
        {
            Name = model.Name;
            Type = model.Type;
        }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id}, {Type})";
    }
}
