using System.Diagnostics;
using Model = Discord.API.Rpc.VoiceShortcut;

namespace Discord.Rpc
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct VoiceShortcut
    {
        public VoiceShortcutType Type { get; }
        public int Code { get; }
        public string Name { get; }

        internal VoiceShortcut(VoiceShortcutType type, int code, string name)
        {
            Type = type;
            Code = code;
            Name = name;
        }
        internal static VoiceShortcut Create(Model model)
        {
            return new VoiceShortcut(model.Type.Value, model.Code.Value, model.Name.Value);
        }

        public override string ToString() => $"{Name}";
        private string DebuggerDisplay => $"{Name} ({Code}, {Type})";
    }
}
