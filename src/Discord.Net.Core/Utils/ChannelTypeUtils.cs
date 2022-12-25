using System.Collections.Generic;

namespace Discord.Utils;

public static class ChannelTypeUtils
{
    public static List<ChannelType> AllChannelTypes()
        => new List<ChannelType>()
        {
            ChannelType.Forum, ChannelType.Category, ChannelType.DM, ChannelType.Group, ChannelType.GuildDirectory,
            ChannelType.News, ChannelType.NewsThread, ChannelType.PrivateThread, ChannelType.PublicThread,
            ChannelType.Stage, ChannelType.Store, ChannelType.Text, ChannelType.Voice
        };
}
