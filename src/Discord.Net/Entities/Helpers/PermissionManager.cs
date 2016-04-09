using Discord.API.Rest;
using Discord.Net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord
{
    internal class PermissionManager
    {
        public struct Member
        {
            public GuildUser User { get; }
            public ChannelPermissions Permissions { get; }

            public Member(GuildUser user, ChannelPermissions permissions)
            {
                User = user;
                Permissions = permissions;
            }
        }

        private readonly GuildChannel _channel;
        private readonly ConcurrentDictionary<ulong, Member> _users;
        private Dictionary<ulong, Overwrite> _rules;

        public IEnumerable<Member> Users => _users.Select(x => x.Value);
        public IEnumerable<Overwrite> Overwrites => _rules.Values;

        public PermissionManager(GuildChannel channel, bool cacheUsers)
        {
            _channel = channel;
            if (cacheUsers)
                _users = new ConcurrentDictionary<ulong, Member>(2, (int)(channel.Guild.UserCount * 1.05));
        }

        public void Update(Model model)
        {
            _rules = model.PermissionOverwrites
                .Select(x => new Overwrite(x))
                .ToDictionary(x => x.TargetId);
            UpdatePermissions();
        }

        public OverwritePermissions? GetOverwrite(IUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            Overwrite rule;
            if (_rules.TryGetValue(user.Id, out rule))
                return rule.Permissions;
            return null;
        }
        public OverwritePermissions? GetOverwrite(Role role)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));

            Overwrite rule;
            if (_rules.TryGetValue(role.Id, out rule))
                return rule.Permissions;
            return null;
        }
        public Task AddOrUpdateOverwrite(IUser user, OverwritePermissions permissions)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return AddOrUpdateOverwrite(user.Id, permissions);
        }
        public Task AddOrUpdateOverwrite(Role role, OverwritePermissions permissions)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));
            return AddOrUpdateOverwrite(role.Id, permissions);
        }
        private Task AddOrUpdateOverwrite(ulong targetId, OverwritePermissions permissions)
        {
            var request = new ModifyChannelPermissionsRequest(_channel.Id, targetId)
            {
                Allow = permissions.AllowValue,
                Deny = permissions.DenyValue
            };
            return _channel.Discord.RestClient.Send(request);
        }
        public Task RemoveOverwrite(IUser user)
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
            try { await _channel.Discord.RestClient.Send(new DeleteChannelPermissionsRequest(_channel.Id, id)).ConfigureAwait(false); }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
        }

        public ChannelPermissions GetPermissions(IUser user)
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
                var perms = new ChannelPermissions();
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
        public void UpdatePermissions(IUser user)
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

        public ChannelPermissions ResolvePermissions(IUser user)
        {
            var permissions = new ChannelPermissions();
            ResolvePermissions(user, ref permissions);
            return permissions;
        }
        private ChannelPermissions ResolvePermissions(GuildUser user)
        {
            var permissions = new ChannelPermissions();
            ResolvePermissions(user, ref permissions);
            return permissions;
        }
        public bool ResolvePermissions(IUser user, ref ChannelPermissions permissions)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            GuildUser guildUser = _channel.GetUser(user.Id);
            if (guildUser == null)
            {
                permissions = ChannelPermissions.None;
                return false;
            }
            else
                return ResolvePermissions(user, ref permissions);
        }
        private bool ResolvePermissions(GuildUser user, ref ChannelPermissions permissions)
        {
            uint newPermissions = 0;

            uint mask = ChannelPermissions.All(_channel.Type).RawValue;
            if (user == user.Guild.Owner)
                newPermissions = mask; //Private messages and owners always have all permissions
            else
            {
                //Start with this user's guild permissions
                newPermissions = user.GuildPermissions.RawValue;
                var rules = _rules;

                Overwrite entry;
                var roles = user.Roles.ToArray();
                if (roles.Length > 0)
                {
                    for (int i = 0; i < roles.Length; i++)
                    {
                        if (rules.TryGetValue(roles[i].Id, out entry))
                            newPermissions &= ~entry.Permissions.DenyValue;
                    }
                    for (int i = 0; i < roles.Length; i++)
                    {
                        if (rules.TryGetValue(roles[i].Id, out entry))
                            newPermissions |= entry.Permissions.AllowValue;
                    }
                }
                if (rules.TryGetValue(user.Id, out entry))
                    newPermissions = (newPermissions & ~entry.Permissions.DenyValue) | entry.Permissions.AllowValue;

                if (PermissionsHelper.HasBit(ref newPermissions, (int)PermissionBit.ManageRolesOrPermissions))
                    newPermissions = mask; //ManageRolesOrPermissions gives all permisions
                else
                {
                    var channelType = _channel.Type;
                    if (channelType == ChannelType.Text && !PermissionsHelper.HasBit(ref newPermissions, (int)PermissionBit.ReadMessages))
                        newPermissions = 0; //No read permission on a text channel removes all other permissions
                    else if (channelType == ChannelType.Voice && !PermissionsHelper.HasBit(ref newPermissions, (int)PermissionBit.Connect))
                        newPermissions = 0; //No connect permissions on a voice channel removes all other permissions
                    else
                        newPermissions &= mask; //Ensure we didnt get any permissions this channel doesnt support (from guildPerms, for example)
                }
            }

            if (newPermissions != permissions.RawValue)
            {
                permissions = new ChannelPermissions(newPermissions);
                return true;
            }
            return false;
        }

        public GuildUser GetUser(ulong id)
        {
            if (_users != null)
            {
                Member member;
                if (_users.TryGetValue(id, out member))
                    return member.User;
            }
            else
            {
                var user = _channel.Guild.GetUser(id);
                if (_channel.Type == ChannelType.Text)
                {
                    if (ResolvePermissions(user).ReadMessages)
                        return user;
                }
                else if (_channel.Type == ChannelType.Voice)
                {
                    if (user.VoiceChannel == _channel)
                        return user;
                }
            }
            return null;
        }
        public IEnumerable<GuildUser> GetMembers()
        {
            if (_users != null)
                return _users.Select(x => x.Value.User);
            else
            {
                var users = _channel.Guild.Users;
                if (_channel.Type == ChannelType.Text)
                {
                    var perms = new ChannelPermissions();
                    return users.Where(x => ResolvePermissions(x, ref perms));
                }
                else if (_channel.Type == ChannelType.Voice)
                    return users.Where(x => x.VoiceChannel == _channel);
            }
            return Enumerable.Empty<GuildUser>();
        }

        public void AddUser(GuildUser user)
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
