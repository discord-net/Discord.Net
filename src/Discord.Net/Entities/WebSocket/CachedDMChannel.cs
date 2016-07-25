using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using MessageModel = Discord.API.Message;
using Model = Discord.API.Channel;

namespace Discord
{
    internal class CachedDMChannel : DMChannel, IDMChannel, ICachedChannel, ICachedMessageChannel, ICachedPrivateChannel
    {
        private readonly MessageManager _messages;

        public new DiscordSocketClient Discord => base.Discord as DiscordSocketClient;
        public new CachedDMUser Recipient => base.Recipient as CachedDMUser;
        public IReadOnlyCollection<ICachedUser> Members => ImmutableArray.Create<ICachedUser>(Discord.CurrentUser, Recipient);
        IReadOnlyCollection<ICachedUser> ICachedPrivateChannel.Recipients => ImmutableArray.Create(Recipient);

        public CachedDMChannel(DiscordSocketClient discord, CachedDMUser recipient, Model model)
            : base(discord, recipient, model)
        {
            if (Discord.MessageCacheSize > 0)
                _messages = new MessageCache(Discord, this);
            else
                _messages = new MessageManager(Discord, this);
        }

        public override Task<IUser> GetUserAsync(ulong id) => Task.FromResult<IUser>(GetUser(id));
        public override Task<IReadOnlyCollection<IUser>> GetUsersAsync() => Task.FromResult<IReadOnlyCollection<IUser>>(Members);
        public ICachedUser GetUser(ulong id)
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
        public CachedMessage AddMessage(ICachedUser author, MessageModel model)
        {
            var msg = new CachedMessage(this, author, model);
            _messages.Add(msg);
            return msg;
        }
        public CachedMessage GetMessage(ulong id)
        {
            return _messages.Get(id);
        }
        public CachedMessage RemoveMessage(ulong id)
        {
            return _messages.Remove(id);
        }

        public CachedDMChannel Clone() => MemberwiseClone() as CachedDMChannel;

        IMessage IMessageChannel.GetCachedMessage(ulong id) => GetMessage(id);
        ICachedUser ICachedMessageChannel.GetUser(ulong id, bool skipCheck) => GetUser(id);
        ICachedChannel ICachedChannel.Clone() => Clone();
    }
}
