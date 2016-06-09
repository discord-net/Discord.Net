using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using MessageModel = Discord.API.Message;
using Model = Discord.API.Channel;

namespace Discord
{
    internal class CachedDMChannel : DMChannel, IDMChannel, ICachedChannel, ICachedMessageChannel
    {
        private readonly MessageCache _messages;

        public new DiscordSocketClient Discord => base.Discord as DiscordSocketClient;
        public new CachedPublicUser Recipient => base.Recipient as CachedPublicUser;
        public IReadOnlyCollection<ICachedUser> Members => ImmutableArray.Create<ICachedUser>(Discord.CurrentUser, Recipient);

        public CachedDMChannel(DiscordSocketClient discord, CachedPublicUser recipient, Model model)
            : base(discord, recipient, model)
        {
            _messages = new MessageCache(Discord, this);
        }

        public override Task<IUser> GetUserAsync(ulong id) => Task.FromResult<IUser>(GetCachedUser(id));
        public override Task<IReadOnlyCollection<IUser>> GetUsersAsync() => Task.FromResult<IReadOnlyCollection<IUser>>(Members);
        public override Task<IReadOnlyCollection<IUser>> GetUsersAsync(int limit, int offset) 
            => Task.FromResult<IReadOnlyCollection<IUser>>(Members.Skip(offset).Take(limit).ToImmutableArray());
        public ICachedUser GetCachedUser(ulong id)
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
        public CachedMessage AddCachedMessage(ICachedUser author, MessageModel model)
        {
            var msg = new CachedMessage(this, author, model);
            _messages.Add(msg);
            return msg;
        }
        public CachedMessage GetCachedMessage(ulong id)
        {
            return _messages.Get(id);
        }
        public CachedMessage RemoveCachedMessage(ulong id)
        {
            return _messages.Remove(id);
        }

        public CachedDMChannel Clone() => MemberwiseClone() as CachedDMChannel;

        IMessage IMessageChannel.GetCachedMessage(ulong id) => GetCachedMessage(id);
        ICachedChannel ICachedChannel.Clone() => Clone();
    }
}
