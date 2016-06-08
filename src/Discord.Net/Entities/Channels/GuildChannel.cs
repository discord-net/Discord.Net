using Discord.API.Rest;
using Discord.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    internal abstract class GuildChannel : SnowflakeEntity, IGuildChannel
    {
        private ConcurrentDictionary<ulong, Overwrite> _overwrites;

        public string Name { get; private set; }
        public int Position { get; private set; }

        public Guild Guild { get; private set; }

        public override DiscordClient Discord => Guild.Discord;

        public GuildChannel(Guild guild, Model model)
            : base(model.Id)
        {
            Guild = guild;

            Update(model, UpdateSource.Creation);
        }
        public virtual void Update(Model model, UpdateSource source)
        {
            if (source == UpdateSource.Rest && IsAttached) return;

            Name = model.Name;
            Position = model.Position;

            var newOverwrites = new ConcurrentDictionary<ulong, Overwrite>();
            for (int i = 0; i < model.PermissionOverwrites.Length; i++)
            {
                var overwrite = model.PermissionOverwrites[i];
                newOverwrites[overwrite.TargetId] = new Overwrite(overwrite);
            }
            _overwrites = newOverwrites;
        }

        public async Task Update()
        {
            if (IsAttached) throw new NotSupportedException();

            var model = await Discord.ApiClient.GetChannel(Id).ConfigureAwait(false);
            Update(model, UpdateSource.Rest);
        }
        public async Task Modify(Action<ModifyGuildChannelParams> func)
        {
            if (func != null) throw new NullReferenceException(nameof(func));

            var args = new ModifyGuildChannelParams();
            func(args);
            var model = await Discord.ApiClient.ModifyGuildChannel(Id, args).ConfigureAwait(false);
            Update(model, UpdateSource.Rest);
        }
        public async Task Delete()
        {
            await Discord.ApiClient.DeleteChannel(Id).ConfigureAwait(false);
        }

        public abstract Task<IGuildUser> GetUser(ulong id);
        public abstract Task<IReadOnlyCollection<IGuildUser>> GetUsers();
        public abstract Task<IReadOnlyCollection<IGuildUser>> GetUsers(int limit, int offset);

        public async Task<IReadOnlyCollection<IInviteMetadata>> GetInvites()
        {
            var models = await Discord.ApiClient.GetChannelInvites(Id).ConfigureAwait(false);
            return models.Select(x => new InviteMetadata(Discord, x)).ToImmutableArray();
        }
        public async Task<IInviteMetadata> CreateInvite(int? maxAge, int? maxUses, bool isTemporary, bool withXkcd)
        {
            var args = new CreateChannelInviteParams
            {
                MaxAge = maxAge ?? 0,
                MaxUses = maxUses ?? 0,
                Temporary = isTemporary,
                XkcdPass = withXkcd
            };
            var model = await Discord.ApiClient.CreateChannelInvite(Id, args).ConfigureAwait(false);
            return new InviteMetadata(Discord, model);
        }

        public OverwritePermissions? GetPermissionOverwrite(IUser user)
        {
            Overwrite value;
            if (_overwrites.TryGetValue(Id, out value))
                return value.Permissions;
            return null;
        }
        public OverwritePermissions? GetPermissionOverwrite(IRole role)
        {
            Overwrite value;
            if (_overwrites.TryGetValue(Id, out value))
                return value.Permissions;
            return null;
        }
        
        public async Task AddPermissionOverwrite(IUser user, OverwritePermissions perms)
        {
            var args = new ModifyChannelPermissionsParams { Allow = perms.AllowValue, Deny = perms.DenyValue };
            await Discord.ApiClient.ModifyChannelPermissions(Id, user.Id, args).ConfigureAwait(false);
            _overwrites[user.Id] = new Overwrite(new API.Overwrite { Allow = perms.AllowValue, Deny = perms.DenyValue, TargetId = user.Id, TargetType = PermissionTarget.User });
        }
        public async Task AddPermissionOverwrite(IRole role, OverwritePermissions perms)
        {
            var args = new ModifyChannelPermissionsParams { Allow = perms.AllowValue, Deny = perms.DenyValue };
            await Discord.ApiClient.ModifyChannelPermissions(Id, role.Id, args).ConfigureAwait(false);
            _overwrites[role.Id] = new Overwrite(new API.Overwrite { Allow = perms.AllowValue, Deny = perms.DenyValue, TargetId = role.Id, TargetType = PermissionTarget.Role });
        }
        public async Task RemovePermissionOverwrite(IUser user)
        {
            await Discord.ApiClient.DeleteChannelPermission(Id, user.Id).ConfigureAwait(false);

            Overwrite value;
            _overwrites.TryRemove(user.Id, out value);
        }
        public async Task RemovePermissionOverwrite(IRole role)
        {
            await Discord.ApiClient.DeleteChannelPermission(Id, role.Id).ConfigureAwait(false);

            Overwrite value;
            _overwrites.TryRemove(role.Id, out value);
        }
        
        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id})";
        
        IGuild IGuildChannel.Guild => Guild;
        IReadOnlyCollection<Overwrite> IGuildChannel.PermissionOverwrites => _overwrites.ToReadOnlyCollection();

        async Task<IUser> IChannel.GetUser(ulong id) => await GetUser(id).ConfigureAwait(false);
        async Task<IReadOnlyCollection<IUser>> IChannel.GetUsers() => await GetUsers().ConfigureAwait(false);
        async Task<IReadOnlyCollection<IUser>> IChannel.GetUsers(int limit, int offset) => await GetUsers(limit, offset).ConfigureAwait(false);
    }
}
