using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using MessageModel = Discord.API.Message;
using Model = Discord.API.Channel;

namespace Discord
{
    internal class SocketDMChannel : DMChannel, IDMChannel, ISocketChannel, ISocketMessageChannel, ISocketPrivateChannel
    {
        internal override bool IsAttached => true;

        private readonly MessageManager _messages;

        public new DiscordSocketClient Discord => base.Discord as DiscordSocketClient;
        public new SocketDMUser Recipient => base.Recipient as SocketDMUser;
        public IReadOnlyCollection<ISocketUser> Users => ImmutableArray.Create<ISocketUser>(Discord.CurrentUser, Recipient);
        IReadOnlyCollection<ISocketUser> ISocketPrivateChannel.Recipients => ImmutableArray.Create(Recipient);

        public SocketDMChannel(DiscordSocketClient discord, SocketDMUser recipient, Model model)
            : base(discord, recipient, model)
        {
            if (Discord.MessageCacheSize > 0)
                _messages = new MessageCache(Discord, this);
            else
                _messages = new MessageManager(Discord, this);
        }

        public override Task<IUser> GetUserAsync(ulong id) => Task.FromResult<IUser>(GetUser(id));
        public override Task<IReadOnlyCollection<IUser>> GetUsersAsync() => Task.FromResult<IReadOnlyCollection<IUser>>(Users);
        public ISocketUser GetUser(ulong id)
        {
            var currentUser = Discord.CurrentUser;
            if (id == Recipient.Id)
                return Recipient;
            else if (id == currentUser.Id)
                return currentUser;
            else
                return null;
        }

        public override async Task<IMessage> GetMessageAsync(ulong id)
        {
            return await _messages.DownloadAsync(id).ConfigureAwait(false);
        }
        public override async Task<IReadOnlyCollection<IMessage>> GetMessagesAsync(int limit)
        {
            return await _messages.DownloadAsync(null, Direction.Before, limit).ConfigureAwait(false);
        }
        public override async Task<IReadOnlyCollection<IMessage>> GetMessagesAsync(ulong fromMessageId, Direction dir, int limit)
        {
            return await _messages.DownloadAsync(fromMessageId, dir, limit).ConfigureAwait(false);
        }
        public SocketMessage AddMessage(ISocketUser author, MessageModel model)
        {
            var msg = new SocketMessage(this, author, model);
            _messages.Add(msg);
            return msg;
        }
        public SocketMessage GetMessage(ulong id)
        {
            return _messages.Get(id);
        }
        public SocketMessage RemoveMessage(ulong id)
        {
            return _messages.Remove(id);
        }

        public SocketDMChannel Clone() => MemberwiseClone() as SocketDMChannel;

        IMessage IMessageChannel.GetCachedMessage(ulong id) => GetMessage(id);
        ISocketUser ISocketMessageChannel.GetUser(ulong id, bool skipCheck) => GetUser(id);
        ISocketChannel ISocketChannel.Clone() => Clone();
    }
}
