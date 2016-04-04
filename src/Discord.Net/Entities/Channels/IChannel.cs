using System.Collections.Generic;

namespace Discord
{
    public interface IChannel : IEntity<ulong>
    {
        /// <summary> Gets the type flags for this channel. </summary>
        ChannelType Type { get; }
        /// <summary> Gets the name of this channel. </summary>
        string Name { get; }
        /// <summary> Gets a collection of all users in this channel. </summary>
        IEnumerable<User> Users { get; }

        /// <summary> Gets a user in this channel with the given id. </summary>
        User GetUser(ulong id);
    }
}
