using System.Diagnostics;
using Model = Discord.API.Message;

namespace Discord.Rest
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    internal class SystemMessage : Message, ISystemMessage
    {        
        public MessageType Type { get; }

        public override DiscordRestClient Discord => (Channel as Entity<ulong>).Discord;

        public SystemMessage(IMessageChannel channel, IUser author, Model model)
            : base(channel, author, model)
        {
            Type = model.Type;
        }
        
        public override string ToString() => Content;
        private string DebuggerDisplay => $"[{Type}] {Author}{(!string.IsNullOrEmpty(Content) ? $": ({Content})" : "")}";
    }
}
