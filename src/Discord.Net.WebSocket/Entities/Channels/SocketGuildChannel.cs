using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.Rest
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    internal abstract class SocketGuildChannel : ISnowflakeEntity, IGuildChannel
    {
        private List<Overwrite> _overwrites; //TODO: Is maintaining a list here too expensive? Is this threadsafe?

        public string Name { get; private set; }
        public int Position { get; private set; }

        public Guild Guild { get; private set; }

        public override DiscordRestClient Discord => Guild.Discord;

        public GuildChannel(Guild guild, Model model)
            : base(model.Id)
        {
            Guild = guild;

            Update(model);
        }
        public virtual void Update(Model model)
        {
            if (source == UpdateSource.Rest && IsAttached) return;

            Name = model.Name.Value;
            Position = model.Position.Value;

            var overwrites = model.PermissionOverwrites.Value;
            var newOverwrites = new List<Overwrite>(overwrites.Length);
            for (int i = 0; i < overwrites.Length; i++)
                newOverwrites.Add(new Overwrite(overwrites[i]));
            _overwrites = newOverwrites;
        }

        public async Task UpdateAsync()
        {
            if (IsAttached) throw new NotSupportedException();

            var model = await Discord.ApiClient.GetChannelAsync(Id).ConfigureAwait(false);
            Update(model, UpdateSource.Rest);
        }
        public async Task ModifyAsync(Action<ModifyGuildChannelParams> func)
        {
            if (func == null) throw new NullReferenceException(nameof(func));

            var args = new ModifyGuildChannelParams();
            func(args);

            if (!args._name.IsSpecified)
                args._name = Name;

            var model = await Discord.ApiClient.ModifyGuildChannelAsync(Id, args).ConfigureAwait(false);
            Update(model, UpdateSource.Rest);
        }
        public async Task DeleteAsync()
        {
            await Discord.ApiClient.DeleteChannelAsync(Id).ConfigureAwait(false);
        }

        public abstract Task<IGuildUser> GetUserAsync(ulong id);
        public abstract Task<IReadOnlyCollection<IGuildUser>> GetUsersAsync();

        public async Task<IReadOnlyCollection<IInviteMetadata>> GetInvitesAsync()
        {
            var models = await Discord.ApiClient.GetChannelInvitesAsync(Id).ConfigureAwait(false);
            return models.Select(x => new InviteMetadata(Discord, x)).ToImmutableArray();
        }
        public async Task<IInviteMetadata> CreateInviteAsync(int? maxAge, int? maxUses, bool isTemporary)
        {
            var args = new CreateChannelInviteParams
            {
                MaxAge = maxAge ?? 0,
                MaxUses = maxUses ?? 0,
                Temporary = isTemporary
            };
            var model = await Discord.ApiClient.CreateChannelInviteAsync(Id, args).ConfigureAwait(false);
            return new InviteMetadata(Discord, model);
        }

        public OverwritePermissions? GetPermissionOverwrite(IUser user)
        {
            for (int i = 0; i < _overwrites.Count; i++)
            {
                if (_overwrites[i].TargetId == user.Id)
                    return _overwrites[i].Permissions;
            }
            return null;
        }
        public OverwritePermissions? GetPermissionOverwrite(IRole role)
        {
            for (int i = 0; i < _overwrites.Count; i++)
            {
                if (_overwrites[i].TargetId == role.Id)
                    return _overwrites[i].Permissions;
            }
            return null;
        }
        
        public async Task AddPermissionOverwriteAsync(IUser user, OverwritePermissions perms)
        {
            var args = new ModifyChannelPermissionsParams { Allow = perms.AllowValue, Deny = perms.DenyValue, Type = "member" };
            await Discord.ApiClient.ModifyChannelPermissionsAsync(Id, user.Id, args).ConfigureAwait(false);
            _overwrites.Add(new Overwrite(new API.Overwrite { Allow = perms.AllowValue, Deny = perms.DenyValue, TargetId = user.Id, TargetType = PermissionTarget.User }));
        }
        public async Task AddPermissionOverwriteAsync(IRole role, OverwritePermissions perms)
        {
            var args = new ModifyChannelPermissionsParams { Allow = perms.AllowValue, Deny = perms.DenyValue, Type = "role" };
            await Discord.ApiClient.ModifyChannelPermissionsAsync(Id, role.Id, args).ConfigureAwait(false);
            _overwrites.Add(new Overwrite(new API.Overwrite { Allow = perms.AllowValue, Deny = perms.DenyValue, TargetId = role.Id, TargetType = PermissionTarget.Role }));
        }
        public async Task RemovePermissionOverwriteAsync(IUser user)
        {
            await Discord.ApiClient.DeleteChannelPermissionAsync(Id, user.Id).ConfigureAwait(false);

            for (int i = 0; i < _overwrites.Count; i++)
            {
                if (_overwrites[i].TargetId == user.Id)
                {
                    _overwrites.RemoveAt(i);
                    return;
                }
            }
        }
        public async Task RemovePermissionOverwriteAsync(IRole role)
        {
            await Discord.ApiClient.DeleteChannelPermissionAsync(Id, role.Id).ConfigureAwait(false);

            for (int i = 0; i < _overwrites.Count; i++)
            {
                if (_overwrites[i].TargetId == role.Id)
                {
                    _overwrites.RemoveAt(i);
                    return;
                }
            }
        }
        
        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id})";
        
        IGuild IGuildChannel.Guild => Guild;
        IReadOnlyCollection<Overwrite> IGuildChannel.PermissionOverwrites => _overwrites.AsReadOnly();

        async Task<IUser> IChannel.GetUserAsync(ulong id) => await GetUserAsync(id).ConfigureAwait(false);
        async Task<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync() => await GetUsersAsync().ConfigureAwait(false);
    }
}
