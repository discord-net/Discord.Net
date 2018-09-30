using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a generic REST-based channel.
    /// </summary>
    public class RestChannel : RestEntity<ulong>, IChannel, IUpdateable
    {
        /// <inheritdoc />
        public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

        internal RestChannel(BaseDiscordClient discord, ulong id)
            : base(discord, id)
        {
        }
        /// <exception cref="InvalidOperationException">Unexpected channel type.</exception>
        internal static RestChannel Create(BaseDiscordClient discord, Model model)
        {
            switch (model.Type)
            {
                case ChannelType.Text:
                case ChannelType.Voice:
                    return RestGuildChannel.Create(discord, new RestGuild(discord, model.GuildId.Value), model);
                case ChannelType.DM:
                case ChannelType.Group:
                    return CreatePrivate(discord, model) as RestChannel;
                case ChannelType.Category:
                    return RestCategoryChannel.Create(discord, new RestGuild(discord, model.GuildId.Value), model);
                default:
                    return new RestChannel(discord, model.Id);
            }
        }
        /// <exception cref="InvalidOperationException">Unexpected channel type.</exception>
        internal static IRestPrivateChannel CreatePrivate(BaseDiscordClient discord, Model model)
        {
            switch (model.Type)
            {
                case ChannelType.DM:
                    return RestDMChannel.Create(discord, model);
                case ChannelType.Group:
                    return RestGroupChannel.Create(discord, model);
                default:
                    throw new InvalidOperationException($"Unexpected channel type: {model.Type}");
            }
        }
        internal virtual void Update(Model model) { }

        /// <inheritdoc />
        public virtual Task UpdateAsync(RequestOptions options = null) => Task.Delay(0);

        //IChannel
        /// <inheritdoc />
        string IChannel.Name => null;

        /// <inheritdoc />
        Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IUser>(null); //Overridden
        /// <inheritdoc />
        IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
            => AsyncEnumerable.Empty<IReadOnlyCollection<IUser>>(); //Overridden
    }
}
