using Discord.Rest;
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
        internal new static SocketSystemMessage Create(DiscordSocketClient discord, SocketUser author, Model model)
        {
            var entity = new SocketSystemMessage(discord, model.Id, model.ChannelId, author);
            entity.Update(model);
            return entity;
        }
        internal override void Update(Model model)
        {
            base.Update(model);

            Type = model.Type;
        }
    }
}
