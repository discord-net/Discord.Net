using System.Collections.Generic;

namespace Discord
{
    public interface IChannel
    {
        /// <summary> Gets the unique identifier for this channel. </summary>
        ulong Id { get; }
        DiscordClient Client { get; }

        /// <summary> Gets the type of this channel. </summary>
        ChannelType Type { get; }
        bool IsText { get; }
        bool IsVoice { get; }
        bool IsPrivate { get; }
        bool IsPublic { get; }

        /// <summary> Gets a collection of all users in this channel. </summary>
        IEnumerable<User> Users { get; }
    }
}
