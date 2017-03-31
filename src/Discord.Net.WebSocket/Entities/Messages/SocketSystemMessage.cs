using System.Diagnostics;
using Model = Discord.API.Message;

namespace Discord.WebSocket
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketSystemMessage : SocketMessage, ISystemMessage
    {
        public MessageType Type { get; private set; }

        internal SocketSystemMessage(DiscordSocketClient discord, ulong id, ISocketMessageChannel channel, SocketUser author)
            : base(discord, id, channel, author, MessageSource.System)
        {
        }
        internal new static SocketSystemMessage Create(DiscordSocketClient discord, ClientState state, SocketUser author, ISocketMessageChannel channel, Model model)
        {
            var entity = new SocketSystemMessage(discord, model.Id, channel, author);
            entity.Update(state, model);
            return entity;
        }
        internal override void Update(ClientState state, Model model)
        {
            base.Update(state, model);

            Type = model.Type;
        }
        
        private string DebuggerDisplay => $"{Author}: {Content} ({Id}, {Type})";
        internal new SocketSystemMessage Clone() => MemberwiseClone() as SocketSystemMessage;
    }
}
