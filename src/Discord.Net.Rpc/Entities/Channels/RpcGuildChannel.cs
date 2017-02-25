using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Model = Discord.API.Rpc.Channel;
using Discord.Rest;

namespace Discord.Rpc
{
    public class RpcGuildChannel : RpcChannel, IGuildChannel
    {
        public ulong GuildId { get; }
        public int Position { get; private set; }

        internal RpcGuildChannel(DiscordRpcClient discord, ulong id, ulong guildId)
            : base(discord, id)
        {
            GuildId = guildId;
        }
        internal new static RpcGuildChannel Create(DiscordRpcClient discord, Model model)
        {
            switch (model.Type)
            {
                case ChannelType.Text:
                    return RpcTextChannel.Create(discord, model);
                case ChannelType.Voice:
                    return RpcVoiceChannel.Create(discord, model);
                default:
                    throw new InvalidOperationException("Unknown guild channel type");
            }
        }
        internal override void Update(Model model)
        {
            base.Update(model);
            if (model.Position.IsSpecified)
                Position = model.Position.Value;
        }

        public Task ModifyAsync(Action<GuildChannelProperties> func, RequestOptions options = null)
            => ChannelHelper.ModifyAsync(this, Discord, func, options);
        public Task DeleteAsync(RequestOptions options = null)
            => ChannelHelper.DeleteAsync(this, Discord, options);

        public Task AddPermissionOverwriteAsync(IUser user, OverwritePermissions perms, RequestOptions options = null)
            => ChannelHelper.AddPermissionOverwriteAsync(this, Discord, user, perms, options);
        public Task AddPermissionOverwriteAsync(IRole role, OverwritePermissions perms, RequestOptions options = null)
            => ChannelHelper.AddPermissionOverwriteAsync(this, Discord, role, perms, options);
        public Task RemovePermissionOverwriteAsync(IUser user, RequestOptions options = null)
            => ChannelHelper.RemovePermissionOverwriteAsync(this, Discord, user, options);
        public Task RemovePermissionOverwriteAsync(IRole role, RequestOptions options = null)
            => ChannelHelper.RemovePermissionOverwriteAsync(this, Discord, role, options);

        public async Task<IReadOnlyCollection<RestInviteMetadata>> GetInvitesAsync(RequestOptions options = null)
            => await ChannelHelper.GetInvitesAsync(this, Discord, options).ConfigureAwait(false);
        public async Task<RestInviteMetadata> CreateInviteAsync(int? maxAge = 3600, int? maxUses = null, bool isTemporary = false, bool isUnique = false, RequestOptions options = null)
            => await ChannelHelper.CreateInviteAsync(this, Discord, maxAge, maxUses, isTemporary, isUnique, options).ConfigureAwait(false);

        public override string ToString() => Name;

        //IGuildChannel
        IGuild IGuildChannel.Guild
        {
            get
            {
                //Always fails
                throw new InvalidOperationException("Unable to return this entity's parent unless it was fetched through that object.");
            }
        }

        async Task<IReadOnlyCollection<IInviteMetadata>> IGuildChannel.GetInvitesAsync(RequestOptions options)
            => await GetInvitesAsync(options).ConfigureAwait(false);
        async Task<IInviteMetadata> IGuildChannel.CreateInviteAsync(int? maxAge, int? maxUses, bool isTemporary, bool isUnique, RequestOptions options)
            => await CreateInviteAsync(maxAge, maxUses, isTemporary, isUnique, options).ConfigureAwait(false);

        IReadOnlyCollection<Overwrite> IGuildChannel.PermissionOverwrites { get { throw new NotSupportedException(); } }
        OverwritePermissions? IGuildChannel.GetPermissionOverwrite(IUser user)
        {
            throw new NotSupportedException();
        }
        OverwritePermissions? IGuildChannel.GetPermissionOverwrite(IRole role)
        {
            throw new NotSupportedException();
        }
        IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuildChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
        {
            throw new NotSupportedException();
        }
        Task<IGuildUser> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
        {
            throw new NotSupportedException();
        }

        //IChannel
        IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
        {
            throw new NotSupportedException();
        }
        Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
        {
            throw new NotSupportedException();
        }
    }
}
