using Discord.API.Rest;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.WebSocket
{
    public abstract class GuildChannel : IGuildChannel
    {
        private ConcurrentDictionary<ulong, Overwrite> _overwrites;
        private ChannelPermissionsCache _permissions;

        /// <inheritdoc />
        public ulong Id { get; }
        /// <summary> Gets the guild this channel is a member of. </summary>
        public Guild Guild { get; }

        /// <inheritdoc />
        public string Name { get; private set; }
        /// <inheritdoc />
        public int Position { get; private set; }

        /// <inheritdoc />
        public DateTime CreatedAt => DateTimeHelper.FromSnowflake(Id);
        /// <inheritdoc />
        public IReadOnlyDictionary<ulong, Overwrite> PermissionOverwrites => _overwrites;
        internal DiscordClient Discord => Guild.Discord;

        internal GuildChannel(Guild guild, Model model)
        {
            Id = model.Id;
            Guild = guild;

            Update(model);
        }
        internal virtual void Update(Model model)
        {
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

        public async Task Modify(Action<ModifyGuildChannelParams> func)
        {
            if (func != null) throw new NullReferenceException(nameof(func));

            var args = new ModifyGuildChannelParams();
            func(args);
            var model = await Discord.BaseClient.ModifyGuildChannel(Id, args).ConfigureAwait(false);
            Update(model);
        }

        /// <summary> Gets a user in this channel with the given id. </summary>
        public async Task<GuildUser> GetUser(ulong id)
        {
            var model = await Discord.BaseClient.GetGuildMember(Guild.Id, id).ConfigureAwait(false);
            if (model != null)
                return new GuildUser(Guild, model);
            return null;
        }
        protected abstract Task<IEnumerable<GuildUser>> GetUsers();

        /// <summary> Gets the permission overwrite for a specific user, or null if one does not exist. </summary>
        public OverwritePermissions? GetPermissionOverwrite(IUser user)
        {
            Overwrite value;
            if (_overwrites.TryGetValue(Id, out value))
                return value.Permissions;
            return null;
        }
        /// <summary> Gets the permission overwrite for a specific role, or null if one does not exist. </summary>
        public OverwritePermissions? GetPermissionOverwrite(IRole role)
        {
            Overwrite value;
            if (_overwrites.TryGetValue(Id, out value))
                return value.Permissions;
            return null;
        }
        /// <summary> Downloads a collection of all invites to this channel. </summary>
        public async Task<IEnumerable<GuildInvite>> GetInvites()
        {
            var models = await Discord.BaseClient.GetChannelInvites(Id).ConfigureAwait(false);
            return models.Select(x => new GuildInvite(Guild, x));
        }

        /// <summary> Adds or updates the permission overwrite for the given user. </summary>
        public async Task AddPermissionOverwrite(IUser user, OverwritePermissions perms)
        {
            var args = new ModifyChannelPermissionsParams { Allow = perms.AllowValue, Deny = perms.DenyValue };
            await Discord.BaseClient.ModifyChannelPermissions(Id, user.Id, args).ConfigureAwait(false);
            _overwrites[user.Id] = new Overwrite(new API.Overwrite { Allow = perms.AllowValue, Deny = perms.DenyValue, TargetId = user.Id, TargetType = PermissionTarget.User });
        }
        /// <summary> Adds or updates the permission overwrite for the given role. </summary>
        public async Task AddPermissionOverwrite(IRole role, OverwritePermissions perms)
        {
            var args = new ModifyChannelPermissionsParams { Allow = perms.AllowValue, Deny = perms.DenyValue };
            await Discord.BaseClient.ModifyChannelPermissions(Id, role.Id, args).ConfigureAwait(false);
            _overwrites[role.Id] = new Overwrite(new API.Overwrite { Allow = perms.AllowValue, Deny = perms.DenyValue, TargetId = role.Id, TargetType = PermissionTarget.Role });
        }
        /// <summary> Removes the permission overwrite for the given user, if one exists. </summary>
        public async Task RemovePermissionOverwrite(IUser user)
        {
            await Discord.BaseClient.DeleteChannelPermission(Id, user.Id).ConfigureAwait(false);

            Overwrite value;
            _overwrites.TryRemove(user.Id, out value);
        }
        /// <summary> Removes the permission overwrite for the given role, if one exists. </summary>
        public async Task RemovePermissionOverwrite(IRole role)
        {
            await Discord.BaseClient.DeleteChannelPermission(Id, role.Id).ConfigureAwait(false);

            Overwrite value;
            _overwrites.TryRemove(role.Id, out value);
        }

        /// <summary> Creates a new invite to this channel. </summary>
        /// <param name="maxAge"> Time (in seconds) until the invite expires. Set to null to never expire. </param>
        /// <param name="maxUses"> The max amount  of times this invite may be used. Set to null to have unlimited uses. </param>
        /// <param name="isTemporary"> If true, a user accepting this invite will be kicked from the guild after closing their client. </param>
        /// <param name="withXkcd"> If true, creates a human-readable link. Not supported if maxAge is set to null. </param>
        public async Task<GuildInvite> CreateInvite(int? maxAge = 1800, int? maxUses = null, bool isTemporary = false, bool withXkcd = false)
        {
            var args = new CreateChannelInviteParams
            {
                MaxAge = maxAge ?? 0,
                MaxUses = maxUses ?? 0,
                Temporary = isTemporary,
                XkcdPass = withXkcd
            };
            var model = await Discord.BaseClient.CreateChannelInvite(Id, args).ConfigureAwait(false);
            return new GuildInvite(Guild, model);
        }

        /// <inheritdoc />
        public async Task Delete()
        {
            await Discord.BaseClient.DeleteChannel(Id).ConfigureAwait(false);
        }
        /// <inheritdoc />
        public async Task Update()
        {
            var model = await Discord.BaseClient.GetChannel(Id).ConfigureAwait(false);
            Update(model);
        }

        IGuild IGuildChannel.Guild => Guild;
        async Task<IGuildInvite> IGuildChannel.CreateInvite(int? maxAge, int? maxUses, bool isTemporary, bool withXkcd)
            => await CreateInvite(maxAge, maxUses, isTemporary, withXkcd).ConfigureAwait(false);
        async Task<IEnumerable<IGuildInvite>> IGuildChannel.GetInvites()
            => await GetInvites().ConfigureAwait(false);
        async Task<IEnumerable<IGuildUser>> IGuildChannel.GetUsers()
            => await GetUsers().ConfigureAwait(false);
        async Task<IGuildUser> IGuildChannel.GetUser(ulong id)
            => await GetUser(id).ConfigureAwait(false);
        async Task<IEnumerable<IUser>> IChannel.GetUsers()
            => await GetUsers().ConfigureAwait(false);
        async Task<IUser> IChannel.GetUser(ulong id)
            => await GetUser(id).ConfigureAwait(false);
    }
}
