using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    public interface IChannel : IEntity<ulong>
    {
        /// <summary> Gets the type flags for this channel. </summary>
        ChannelType Type { get; }
        /// <summary> Gets the name of this channel. </summary>
        string Name { get; }

        /// <summary> Gets a user in this channel with the given id. </summary>
        Task<IUser> GetUser(ulong id);
        /// <summary> Gets a collection of all users in this channel. </summary>
        Task<IEnumerable<IUser>> GetUsers();
    }
}
