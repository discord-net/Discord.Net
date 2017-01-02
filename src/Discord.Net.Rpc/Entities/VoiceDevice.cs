using System.Diagnostics;
using Model = Discord.API.Rpc.VoiceDevice;

namespace Discord.Rpc
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct VoiceDevice
    {
        public string Id { get; }
        public string Name { get; }

        internal VoiceDevice(string id, string name)
        {
            Id = id;
            Name = name;
        }
        internal static VoiceDevice Create(Model model)
        {
            return new VoiceDevice(model.Id, model.Name);
        }

        public override string ToString() => $"{Name}";
        private string DebuggerDisplay => $"{Name} ({Id})";
    }
}
