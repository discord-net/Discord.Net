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
    public abstract class RestGuildChannel : RestChannel, IGuildChannel, IUpdateable
    {
        private ImmutableArray<Overwrite> _overwrites;

        public IReadOnlyCollection<Overwrite> PermissionOverwrites => _overwrites;

        internal IGuild Guild { get; }
        public ulong GuildId => Guild.Id;

        public string Name { get; private set; }
        public int Position { get; private set; }

        internal RestGuildChannel(BaseDiscordClient discord, IGuild guild, ulong id)
            : base(discord, id)
        {
            Guild = guild;
        }
        internal static RestGuildChannel Create(BaseDiscordClient discord, IGuild guild, Model model)
        {
            switch (model.Type)
            {
                case ChannelType.Text:
                    return RestTextChannel.Create(discord, guild, model);
                case ChannelType.Voice:
                    return RestVoiceChannel.Create(discord, guild, model);
                default:
                    throw new InvalidOperationException("Unknown guild channel type");
            }
        }
        internal override void Update(Model model)
        {
            Name = model.Name.Value;
            Position = model.Position.Value;

            var overwrites = model.PermissionOverwrites.Value;
            var newOverwrites = ImmutableArray.CreateBuilder<Overwrite>(overwrites.Length);
            for (int i = 0; i < overwrites.Length; i++)
                newOverwrites.Add(new Overwrite(overwrites[i]));
            _overwrites = newOverwrites.ToImmutable();
        }

        public override async Task UpdateAsync()
            => Update(await ChannelHelper.GetAsync(this, Discord));
        public Task ModifyAsync(Action<ModifyGuildChannelParams> func)
            => ChannelHelper.ModifyAsync(this, Discord, func);
        public Task DeleteAsync()
            => ChannelHelper.DeleteAsync(this, Discord);
        
        public OverwritePermissions? GetPermissionOverwrite(IUser user)
        {
            for (int i = 0; i < _overwrites.Length; i++)
            {
                if (_overwrites[i].TargetId == user.Id)
                    return _overwrites[i].Permissions;
            }
            return null;
        }
        public OverwritePermissions? GetPermissionOverwrite(IRole role)
        {
            for (int i = 0; i < _overwrites.Length; i++)
            {
                if (_overwrites[i].TargetId == role.Id)
                    return _overwrites[i].Permissions;
            }
            return null;
        }
        public async Task AddPermissionOverwriteAsync(IUser user, OverwritePermissions perms)
        {
            await ChannelHelper.AddPermissionOverwriteAsync(this, Discord, user, perms).ConfigureAwait(false);
            _overwrites = _overwrites.Add(new Overwrite(new API.Overwrite { Allow = perms.AllowValue, Deny = perms.DenyValue, TargetId = user.Id, TargetType = PermissionTarget.User }));
        }
        public async Task AddPermissionOverwriteAsync(IRole role, OverwritePermissions perms)
        {
            await ChannelHelper.AddPermissionOverwriteAsync(this, Discord, role, perms).ConfigureAwait(false);
            _overwrites.Add(new Overwrite(new API.Overwrite { Allow = perms.AllowValue, Deny = perms.DenyValue, TargetId = role.Id, TargetType = PermissionTarget.Role }));
        }
        public async Task RemovePermissionOverwriteAsync(IUser user)
        {
            await ChannelHelper.RemovePermissionOverwriteAsync(this, Discord, user).ConfigureAwait(false);

            for (int i = 0; i < _overwrites.Length; i++)
            {
                if (_overwrites[i].TargetId == user.Id)
                {
                    _overwrites = _overwrites.RemoveAt(i);
                    return;
                }
            }
        }
        public async Task RemovePermissionOverwriteAsync(IRole role)
        {
            await ChannelHelper.RemovePermissionOverwriteAsync(this, Discord, role).ConfigureAwait(false);

            for (int i = 0; i < _overwrites.Length; i++)
            {
                if (_overwrites[i].TargetId == role.Id)
                {
                    _overwrites = _overwrites.RemoveAt(i);
                    return;
                }
            }
        }

        public async Task<IReadOnlyCollection<RestInviteMetadata>> GetInvitesAsync()
            => await ChannelHelper.GetInvitesAsync(this, Discord);
        public async Task<RestInviteMetadata> CreateInviteAsync(int? maxAge = 3600, int? maxUses = null, bool isTemporary = true)
            => await ChannelHelper.CreateInviteAsync(this, Discord, maxAge, maxUses, isTemporary);
        
        //IGuildChannel
        async Task<IReadOnlyCollection<IInviteMetadata>> IGuildChannel.GetInvitesAsync()
            => await GetInvitesAsync();
        async Task<IInviteMetadata> IGuildChannel.CreateInviteAsync(int? maxAge, int? maxUses, bool isTemporary)
            => await CreateInviteAsync(maxAge, maxUses, isTemporary);
        
        OverwritePermissions? IGuildChannel.GetPermissionOverwrite(IRole role) 
            => GetPermissionOverwrite(role);
        OverwritePermissions? IGuildChannel.GetPermissionOverwrite(IUser user)
            => GetPermissionOverwrite(user);
        async Task IGuildChannel.AddPermissionOverwriteAsync(IRole role, OverwritePermissions permissions) 
            => await AddPermissionOverwriteAsync(role, permissions);
        async Task IGuildChannel.AddPermissionOverwriteAsync(IUser user, OverwritePermissions permissions) 
            => await AddPermissionOverwriteAsync(user, permissions);
        async Task IGuildChannel.RemovePermissionOverwriteAsync(IRole role) 
            => await RemovePermissionOverwriteAsync(role);
        async Task IGuildChannel.RemovePermissionOverwriteAsync(IUser user) 
            => await RemovePermissionOverwriteAsync(user);
        
        IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuildChannel.GetUsersAsync(CacheMode mode)
            => ImmutableArray.Create<IReadOnlyCollection<IGuildUser>>().ToAsyncEnumerable(); //Overriden in Text/Voice //TODO: Does this actually override?
        Task<IGuildUser> IGuildChannel.GetUserAsync(ulong id, CacheMode mode)
            => Task.FromResult<IGuildUser>(null); //Overriden in Text/Voice //TODO: Does this actually override?

        //IChannel
        IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode)
            => ImmutableArray.Create<IReadOnlyCollection<IUser>>().ToAsyncEnumerable(); //Overriden in Text/Voice //TODO: Does this actually override?
        Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode)
            => Task.FromResult<IUser>(null); //Overriden in Text/Voice //TODO: Does this actually override?
    }
}
