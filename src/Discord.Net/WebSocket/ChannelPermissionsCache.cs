using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Discord.WebSocket
{
    internal struct ChannelMember
    {
        public GuildUser User { get; }
        public ChannelPermissions Permissions { get; }

        public ChannelMember(GuildUser user, ChannelPermissions permissions)
        {
            User = user;
            Permissions = permissions;
        }
    }

    internal class ChannelPermissionsCache
    {
        private readonly GuildChannel _channel;
        private readonly ConcurrentDictionary<ulong, ChannelMember> _users;

        public IEnumerable<ChannelMember> Members => _users.Select(x => x.Value);

        public ChannelPermissionsCache(GuildChannel channel)
        {
            _channel = channel;
            _users = new ConcurrentDictionary<ulong, ChannelMember>(1, (int)(_channel.Guild.UserCount * 1.05));
        }
        
        public ChannelMember? Get(ulong id)
        {
            ChannelMember member;
            if (_users.TryGetValue(id, out member))
                return member;
            return null;
        }
        public void Add(GuildUser user)
        {
            _users[user.Id] = new ChannelMember(user, new ChannelPermissions(PermissionHelper.Resolve(user, _channel)));
        }
        public void Remove(GuildUser user)
        {
            ChannelMember member;
            _users.TryRemove(user.Id, out member);
        }

        public void UpdateAll()
        {
            foreach (var pair in _users)
            {
                var member = pair.Value;
                var newPerms = PermissionHelper.Resolve(member.User, _channel);
                if (newPerms != member.Permissions.RawValue)
                    _users[pair.Key] = new ChannelMember(member.User, new ChannelPermissions(newPerms));
            }
        }
        public void Update(GuildUser user)
        {
            ChannelMember member;
            if (_users.TryGetValue(user.Id, out member))
            {
                var newPerms = PermissionHelper.Resolve(user, _channel);
                if (newPerms != member.Permissions.RawValue)
                    _users[user.Id] = new ChannelMember(user, new ChannelPermissions(newPerms));
            }
        }
    }
}
