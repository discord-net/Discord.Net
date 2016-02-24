using Discord.API.Client.Rest;
using Discord.Net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using APIChannel = Discord.API.Client.Channel;

namespace Discord
{
    internal class PermissionManager
    {
        public struct Member
        {
            public User User { get; }
            public ChannelPermissions Permissions { get; }

            public Member(User user, ChannelPermissions permissions)
            {
                User = user;
                Permissions = permissions;
            }
        }

        private readonly PublicChannel _channel;
        private readonly ConcurrentDictionary<ulong, Member> _users;
        private Dictionary<ulong, Channel.PermissionRule> _rules;

        public IEnumerable<Member> Users => _users.Select(x => x.Value);
        public IEnumerable<Channel.PermissionRule> Rules => _rules.Values;

        public PermissionManager(PublicChannel channel, APIChannel model, int initialSize = -1)
        {
            _channel = channel;
            if (initialSize >= 0)
                _users = new ConcurrentDictionary<ulong, Member>(2, initialSize);
            Update(model);
        }

        public void Update(APIChannel model)
        {
            _rules = model.PermissionOverwrites
                .Select(x => new Channel.PermissionRule(EnumConverters.ToPermissionTarget(x.Type), x.Id, x.Allow, x.Deny))
                .ToDictionary(x => x.TargetId);
            UpdatePermissions();
        }

        public ChannelTriStatePermissions? GetOverwrite(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            Channel.PermissionRule rule;
            if (_rules.TryGetValue(user.Id, out rule))
                return rule.Permissions;
            return null;
        }
        public ChannelTriStatePermissions? GetOverwrite(Role role)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));

            Channel.PermissionRule rule;
            if (_rules.TryGetValue(role.Id, out rule))
                return rule.Permissions;
            return null;
        }
        public Task AddOrUpdateOverwrite(User user, ChannelTriStatePermissions permissions)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return AddOrUpdateOverwrite(user.Id, PermissionTarget.User, permissions);
        }
        public Task AddOrUpdateOverwrite(Role role, ChannelTriStatePermissions permissions)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));
            return AddOrUpdateOverwrite(role.Id, PermissionTarget.Role, permissions);
        }
        private Task AddOrUpdateOverwrite(ulong id, PermissionTarget type, ChannelTriStatePermissions permissions)
        {
            var request = new AddOrUpdateChannelPermissionsRequest(id)
            {
                TargetId = id,
                TargetType = EnumConverters.ToString(type),
                Allow = permissions.AllowValue,
                Deny = permissions.DenyValue
            };
            return _channel.Client.ClientAPI.Send(request);
        }
        public Task RemoveOverwrite(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return RemoveOverwrite(user.Id);
        }
        public Task RemoveOverwrite(Role role)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));
            return RemoveOverwrite(role.Id);
        }
        private async Task RemoveOverwrite(ulong id)
        {
            try { await _channel.Client.ClientAPI.Send(new RemoveChannelPermissionsRequest(_channel.Id, id)).ConfigureAwait(false); }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
        }

        public ChannelPermissions GetPermissions(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            if (_users != null)
            {
                Member member;
                if (_users.TryGetValue(user.Id, out member))
                    return member.Permissions;
                else
                    return ChannelPermissions.None;
            }
            else
            {
                ChannelPermissions perms = new ChannelPermissions();
                ResolvePermissions(user, ref perms);
                return perms;
            }
        }
        public void UpdatePermissions()
        {
            if (_users != null)
            {
                foreach (var pair in _users)
                {
                    var member = pair.Value;
                    var perms = member.Permissions;
                    if (ResolvePermissions(member.User, ref perms))
                        _users[pair.Key] = new Member(member.User, perms);
                }
            }
        }
        public void UpdatePermissions(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            if (_users != null)
            {
                Member member;
                if (_users.TryGetValue(user.Id, out member))
                {
                    var perms = member.Permissions;
                    if (ResolvePermissions(member.User, ref perms))
                        _users[user.Id] = new Member(member.User, perms);
                }
            }
        }
        public bool ResolvePermissions(User user, ref ChannelPermissions permissions)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            uint newPermissions = 0;
            var server = user.Server;

            var mask = ChannelPermissions.All(_channel.Type).RawValue;
            if (_channel.IsPrivate || user.IsOwner)
                newPermissions = mask; //Private messages and owners always have all permissions
            else
            {
                //Start with this user's server permissions
                newPermissions = server.GetPermissions(user).RawValue;
                var rules = _rules;

                Channel.PermissionRule rule;
                var roles = user.Roles.ToArray();
                if (roles.Length > 0)
                {
                    for (int i = 0; i < roles.Length; i++)
                    {
                        if (rules.TryGetValue(roles[i].Id, out rule))
                            newPermissions &= ~rule.Permissions.DenyValue;
                    }
                    for (int i = 0; i < roles.Length; i++)
                    {
                        if (rules.TryGetValue(roles[i].Id, out rule))
                            newPermissions |= rule.Permissions.AllowValue;
                    }
                }
                if (rules.TryGetValue(user.Id, out rule))
                    newPermissions = (newPermissions & ~rule.Permissions.DenyValue) | rule.Permissions.AllowValue;

                if (newPermissions.HasBit((byte)PermissionBits.ManageRolesOrPermissions))
                    newPermissions = mask; //ManageRolesOrPermissions gives all permisions
                else if (_channel.IsText && !newPermissions.HasBit((byte)PermissionBits.ReadMessages))
                    newPermissions = 0; //No read permission on a text channel removes all other permissions
                else if (_channel.IsVoice && !newPermissions.HasBit((byte)PermissionBits.Connect))
                    newPermissions = 0; //No connect permissions on a voice channel removes all other permissions
                else
                    newPermissions &= mask; //Ensure we didnt get any permissions this channel doesnt support (from serverPerms, for example)
            }

            if (newPermissions != permissions.RawValue)
            {
                permissions = new ChannelPermissions(newPermissions);
                return true;
            }
            return false;
        }

        public void AddUser(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            if (_users != null)
            {
                var perms = new ChannelPermissions();
                ResolvePermissions(user, ref perms);
                var member = new Member(user, ChannelPermissions.None);
                _users[user.Id] = new Member(user, ChannelPermissions.None);
            }
        }
        public void RemoveUser(ulong id)
        {
            Member ignored;
            if (_users != null)
                _users.TryRemove(id, out ignored);
        }
    }
}
