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
    public abstract class RestChannel : RestEntity<ulong>, IChannel, IUpdateable
    {
        internal RestChannel(DiscordClient discord, ulong id)
            : base(discord, id)
        {
        }
        internal static RestChannel Create(DiscordClient discord, Model model)
        {
            switch (model.Type)
            {
                case ChannelType.Text:
                    return RestTextChannel.Create(discord, model);
                case ChannelType.Voice:
                    return RestVoiceChannel.Create(discord, model);
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
        IReadOnlyCollection<IUser> IChannel.CachedUsers => ImmutableArray.Create<IUser>();

        IUser IChannel.GetCachedUser(ulong id)
            => null;
        Task<IUser> IChannel.GetUserAsync(ulong id)
            => Task.FromResult<IUser>(null);
        IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync()
            => ImmutableArray.Create<IReadOnlyCollection<IUser>>().ToAsyncEnumerable();
    }
}
