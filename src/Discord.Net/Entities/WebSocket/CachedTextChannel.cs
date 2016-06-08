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

        public IReadOnlyCollection<IGuildUser> Members
            => Guild.Members.Where(x => Permissions.GetValue(Permissions.ResolveChannel(x, this, x.GuildPermissions.RawValue), ChannelPermission.ReadMessages)).ToImmutableArray();

        public CachedTextChannel(CachedGuild guild, Model model)
            : base(guild, model)
        {
            _messages = new MessageCache(Discord, this);
        }

        public override Task<IGuildUser> GetUser(ulong id) => Task.FromResult(GetCachedUser(id));
        public override Task<IReadOnlyCollection<IGuildUser>> GetUsers() => Task.FromResult(Members);
        public override Task<IReadOnlyCollection<IGuildUser>> GetUsers(int limit, int offset)
            => Task.FromResult<IReadOnlyCollection<IGuildUser>>(Members.Skip(offset).Take(limit).ToImmutableArray());
        public IGuildUser GetCachedUser(ulong id)
        {
            var user = Guild.GetCachedUser(id);
            if (user != null && Permissions.GetValue(Permissions.ResolveChannel(user, this, user.GuildPermissions.RawValue), ChannelPermission.ReadMessages))
                return user;
            return null;
        }

        public override async Task<IMessage> GetMessage(ulong id)
        {
            return await _messages.Download(id).ConfigureAwait(false);
        }
        public override async Task<IReadOnlyCollection<IMessage>> GetMessages(int limit = DiscordConfig.MaxMessagesPerBatch)
        {
            return await _messages.Download(null, Direction.Before, limit).ConfigureAwait(false);
        }
        public override async Task<IReadOnlyCollection<IMessage>> GetMessages(ulong fromMessageId, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch)
        {
            return await _messages.Download(fromMessageId, dir, limit).ConfigureAwait(false);
        }

        public CachedMessage AddCachedMessage(IUser author, MessageModel model)
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

        public CachedTextChannel Clone() => MemberwiseClone() as CachedTextChannel;

        IReadOnlyCollection<IUser> ICachedMessageChannel.Members => Members;

        IMessage IMessageChannel.GetCachedMessage(ulong id) => GetCachedMessage(id);
        IUser ICachedMessageChannel.GetCachedUser(ulong id) => GetCachedUser(id);
    }
}
