using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    public interface IChannel : IEntity<ulong>
    {
        /// <summary> Gets the type flags for this channel. </summary>
        ChannelType Type { get; }
        /// <summary> Gets whether this is a text channel. </summary>
        bool IsText { get; }
        /// <summary> Gets whether this is a voice channel. </summary>
        bool IsVoice { get; }
        /// <summary> Gets whether this is a private channel. </summary>
        bool IsPrivate { get; }
        /// <summary> Gets whether this is a public channel. </summary>
        bool IsPublic { get; }

        /// <summary> Gets a collection of all users in this channel. </summary>
        Task<IEnumerable<User>> GetUsers();
    }
}
