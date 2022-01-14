using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public static class ChannelExtensions
    {
        /// <summary>
        ///     Attempts to get the <see cref="ChannelType"/> based off of the channel's interfaces.
        /// </summary>
        /// <param name="channel">The channel to get the type of.</param>
        /// <returns>The <see cref="ChannelType"/> of the channel if found, otherwise <see langword="null"/>.</returns>
        public static ChannelType? GetChannelType(this IChannel channel)
        {
            switch (channel)
            {
                case IStageChannel:
                    return ChannelType.Stage;

                case IThreadChannel thread:
                    return thread.Type switch
                    {
                        ThreadType.NewsThread => ChannelType.NewsThread,
                        ThreadType.PrivateThread => ChannelType.PrivateThread,
                        ThreadType.PublicThread => ChannelType.PublicThread,
                        _ => null,
                    };

                case ICategoryChannel:
                    return ChannelType.Category;

                case IDMChannel:
                    return ChannelType.DM;

                case IGroupChannel:
                    return ChannelType.Group;

                case INewsChannel:
                    return ChannelType.News;

                case IVoiceChannel:
                    return ChannelType.Voice;

                case ITextChannel:
                    return ChannelType.Text;
            }

            return null;
        }
    }
}
