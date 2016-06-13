using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using MessageModel = Discord.API.Message;
using Model = Discord.API.Channel;

namespace Discord
{
    internal class CachedTextChannel : TextChannel, ICachedGuildChannel, ICachedMessageChannel
    {
        private readonly MessageCache _messages;

        public new DiscordSocketClient Discord => base.Discord as DiscordSocketClient;
        public new CachedGuild Guild => base.Guild as CachedGuild;

        public IReadOnlyCollection<CachedGuildUser> Members
            => Guild.Members.Where(x => Permissions.GetValue(Permissions.ResolveChannel(x, this, x.GuildPermissions.RawValue), ChannelPermission.ReadMessages)).ToImmutableArray();

        public CachedTextChannel(CachedGuild guild, Model model)
            : base(guild, model)
        {
            _messages = new MessageCache(Discord, this);
        }

        public override Task<IGuildUser> GetUserAsync(ulong id) => Task.FromResult<IGuildUser>(GetUser(id));
        public override Task<IReadOnlyCollection<IGuildUser>> GetUsersAsync() => Task.FromResult<IReadOnlyCollection<IGuildUser>>(Members);
        public override Task<IReadOnlyCollection<IGuildUser>> GetUsersAsync(int limit, int offset)
            => Task.FromResult<IReadOnlyCollection<IGuildUser>>(Members.Skip(offset).Take(limit).ToImmutableArray());
        public CachedGuildUser GetUser(ulong id, bool skipCheck = false)
        {
            var user = Guild.GetUser(id);
            if (user != null && !skipCheck)
            {
                ulong perms = Permissions.ResolveChannel(user, this, user.GuildPermissions.RawValue);
                if (Permissions.GetValue(perms, ChannelPermission.ReadMessages))
                    return user;
            }
            return null;
        }

        public override async Task<IMessage> GetMessageAsync(ulong id)
        {
            return await _messages.DownloadAsync(id).ConfigureAwait(false);
        }
        public override async Task<IReadOnlyCollection<IMessage>> GetMessagesAsync(int limit = DiscordConfig.MaxMessagesPerBatch)
        {
            return await _messages.DownloadAsync(null, Direction.Before, limit).ConfigureAwait(false);
        }
        public override async Task<IReadOnlyCollection<IMessage>> GetMessagesAsync(ulong fromMessageId, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch)
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

        public CachedTextChannel Clone() => MemberwiseClone() as CachedTextChannel;

        IReadOnlyCollection<ICachedUser> ICachedMessageChannel.Members => Members;

        IMessage IMessageChannel.GetCachedMessage(ulong id) => GetMessage(id);
        ICachedUser ICachedMessageChannel.GetUser(ulong id, bool skipCheck) => GetUser(id, skipCheck);
        ICachedChannel ICachedChannel.Clone() => Clone();
    }
}
