using System.Diagnostics;
using Model = Discord.API.Rpc.ChannelCreatedEvent;

namespace Discord.Rpc
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RpcChannel
    {
        public ulong Id { get; }
        public string Name { get; set; }
        public ChannelType Type { get; set; }

        internal RpcChannel(ulong id)
        {
            Id = id;
        }
        internal static RpcChannel Create(Model model)
        {
            var entity = new RpcChannel(model.Id);
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
