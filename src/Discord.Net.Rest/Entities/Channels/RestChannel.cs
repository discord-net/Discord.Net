using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.Rest
{
    public abstract class RestChannel : RestEntity<ulong>, IChannel, IUpdateable
    {
        internal RestChannel(BaseDiscordClient discord, ulong id)
            : base(discord, id)
        {
        }
        internal static RestChannel Create(BaseDiscordClient discord, Model model)
        {
            switch (model.Type)
            {
                case ChannelType.Text:
                    return RestTextChannel.Create(discord, model);
                case ChannelType.Voice:
                    return RestVoiceChannel.Create(discord, model);
                case ChannelType.DM:
                case ChannelType.Group:
                    return CreatePrivate(discord, model) as RestChannel;
                default:
                    throw new InvalidOperationException($"Unexpected channel type: {model.Type}");
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
        internal abstract void Update(Model model);

        public abstract Task UpdateAsync();

        //IChannel        
        Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode)
            => Task.FromResult<IUser>(null); //Overriden
        IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode)
            => AsyncEnumerable.Empty<IReadOnlyCollection<IUser>>(); //Overriden
    }
}
