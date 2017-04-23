using Model = Discord.API.Gateway.Reaction;

namespace Discord.WebSocket
{
    public class SocketReaction : IReaction
    {
        public ulong UserId { get; }
        public Optional<IUser> User { get; }
        public ulong MessageId { get; }
        public Optional<SocketUserMessage> Message { get; }
        public ISocketMessageChannel Channel { get; }
        public IEmote Emote { get; }

        internal SocketReaction(ISocketMessageChannel channel, ulong messageId, Optional<SocketUserMessage> message, ulong userId, Optional<IUser> user, IEmote emoji)
        {
            Channel = channel;
            MessageId = messageId;
            Message = message;
            UserId = userId;
            User = user;
            Emote = emoji;
        }
        internal static SocketReaction Create(Model model, ISocketMessageChannel channel, Optional<SocketUserMessage> message, Optional<IUser> user)
        {
            IEmote emote;
            if (model.Emoji.Id.HasValue)
                emote = new Emote(model.Emoji.Id.Value, model.Emoji.Name);
            else
                emote = new Emoji(model.Emoji.Name);
            return new SocketReaction(channel, model.MessageId, message, model.UserId, user, emote);
        }
    }
}
