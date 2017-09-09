using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.Rest
{
    public class RestChannel : RestEntity<ulong>, IChannel, IUpdateable
    {
        public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

        internal RestChannel(BaseDiscordClient discord, ulong id)
            : base(discord, id)
        {
        }
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
                default:
                    return new RestChannel(discord, model.Id);
            }
        }
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

        public virtual Task UpdateAsync(RequestOptions options = null) => Task.Delay(0);

        //IChannel
        string IChannel.Name => null;
        bool IChannel.IsNsfw => ChannelHelper.IsNsfw(this);

        Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IUser>(null); //Overriden
        IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
            => AsyncEnumerable.Empty<IReadOnlyCollection<IUser>>(); //Overriden
    }
}
