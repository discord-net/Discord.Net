using Model = Discord.API.Message;

namespace Discord.WebSocket
{
    internal class SocketSystemMessage : SocketMessage, ISystemMessage
    {
        public MessageType Type { get; private set; }

        internal SocketSystemMessage(DiscordSocketClient discord, ulong id, ulong channelId, SocketUser author)
            : base(discord, id, channelId, author)
        {
        }
        internal new static SocketSystemMessage Create(DiscordSocketClient discord, ClientState state, SocketUser author, Model model)
        {
            var entity = new SocketSystemMessage(discord, model.Id, model.ChannelId, author);
            entity.Update(state, model);
            return entity;
        }
        internal override void Update(ClientState state, Model model)
        {
            base.Update(state, model);

            Type = model.Type;
        }

        public override string ToString() => Content;
        private string DebuggerDisplay => $"{Author}: {Content} ({Id}, {Type})";
        internal new SocketSystemMessage Clone() => MemberwiseClone() as SocketSystemMessage;
    }
}
