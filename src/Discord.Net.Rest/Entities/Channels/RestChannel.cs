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
        #region RestChannel

        /// <inheritdoc />
        public ChannelType ChannelType { get; internal set; }

        /// <inheritdoc />
        public virtual DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

        internal RestChannel(BaseDiscordClient discord, ulong id)
            : base(discord, id)
        {
        }
        /// <exception cref="InvalidOperationException">Unexpected channel type.</exception>
        internal static RestChannel Create(BaseDiscordClient discord, Model model)
        {
            return model.Type switch
            {
                ChannelType.News or
                ChannelType.Text or
                ChannelType.Voice or
                ChannelType.Stage or
                ChannelType.NewsThread or
                ChannelType.PrivateThread or
                ChannelType.PublicThread or
                ChannelType.Forum
                    => RestGuildChannel.Create(discord, new RestGuild(discord, model.GuildId.Value), model),
                ChannelType.DM or ChannelType.Group => CreatePrivate(discord, model) as RestChannel,
                ChannelType.Category => RestCategoryChannel.Create(discord, new RestGuild(discord, model.GuildId.Value), model),
                _ => new RestChannel(discord, model.Id),
            };
        }

        internal static RestChannel Create(BaseDiscordClient discord, Model model, IGuild guild)
        {
            return model.Type switch
            {
                ChannelType.News or
                ChannelType.Text or
                ChannelType.Voice or
                ChannelType.Stage or
                ChannelType.NewsThread or
                ChannelType.PrivateThread or
                ChannelType.PublicThread or
                ChannelType.Forum or
                ChannelType.Media
                    => RestGuildChannel.Create(discord, guild, model),
                ChannelType.DM or ChannelType.Group => CreatePrivate(discord, model) as RestChannel,
                ChannelType.Category => RestCategoryChannel.Create(discord, guild, model),
                _ => new RestChannel(discord, model.Id),
            };
        }
        /// <exception cref="InvalidOperationException">Unexpected channel type.</exception>
        internal static IRestPrivateChannel CreatePrivate(BaseDiscordClient discord, Model model)
        {
            return model.Type switch
            {
                ChannelType.DM => RestDMChannel.Create(discord, model),
                ChannelType.Group => RestGroupChannel.Create(discord, model),
                _ => throw new InvalidOperationException($"Unexpected channel type: {model.Type}"),
            };
        }

        internal virtual void Update(Model model)
        {
            ChannelType = model.Type;
        }

        /// <inheritdoc />
        public virtual Task UpdateAsync(RequestOptions options = null) => Task.Delay(0);
        #endregion

        #region IChannel
        /// <inheritdoc />
        string IChannel.Name => null;

        /// <inheritdoc />
        Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IUser>(null); //Overridden
        /// <inheritdoc />
        IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
            => AsyncEnumerable.Empty<IReadOnlyCollection<IUser>>(); //Overridden
        #endregion
    }
}
