using Discord.Gateway.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway
{
    public abstract class SocketGuildChannel : SocketChannel, ICacheableEntity<ulong, IGuildChannelModel>, IGuildChannel
    {
        public int Position
            => Model.Position;

        public ChannelFlags Flags
            => Model.Flags;

        public GuildCacheable Guild { get; }

        public IReadOnlyCollection<Overwrite> PermissionOverwrites => throw new NotImplementedException(); // TODO

        protected abstract override IGuildChannelModel Model { get; }


        internal SocketGuildChannel(DiscordGatewayClient discord, ulong guildId, IGuildChannelModel model)
            : base(discord, model)
        {
            Guild = new(guildId, discord, discord.State.Guilds.ProvideSpecific(guildId));
        }

        IGuildChannelModel ICacheableEntity<ulong, IGuildChannelModel>.GetModel()
            => Model;

        void ICacheableEntity<ulong, IGuildChannelModel>.Update(IGuildChannelModel model)
            => Update(model);

        public Task ModifyAsync(Action<GuildChannelProperties> func, RequestOptions? options = null) => throw new NotImplementedException();
        public OverwritePermissions? GetPermissionOverwrite(IRole role) => throw new NotImplementedException();
        public OverwritePermissions? GetPermissionOverwrite(IUser user) => throw new NotImplementedException();
        public Task RemovePermissionOverwriteAsync(IRole role, RequestOptions? options = null) => throw new NotImplementedException();
        public Task RemovePermissionOverwriteAsync(IUser user, RequestOptions? options = null) => throw new NotImplementedException();
        public Task AddPermissionOverwriteAsync(IRole role, OverwritePermissions permissions, RequestOptions? options = null) => throw new NotImplementedException();
        public Task AddPermissionOverwriteAsync(IUser user, OverwritePermissions permissions, RequestOptions? options = null) => throw new NotImplementedException();
        IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuildChannel.GetUsersAsync(CacheMode mode, RequestOptions? options) => throw new NotImplementedException();
        Task<IGuildUser> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions? options) => throw new NotImplementedException();
        public Task DeleteAsync(RequestOptions? options = null) => throw new NotImplementedException();


        IEntitySource<IGuild, ulong> IGuildChannel.Guild => Guild;
    }
}
