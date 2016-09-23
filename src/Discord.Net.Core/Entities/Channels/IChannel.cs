using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    public interface IChannel : ISnowflakeEntity
    {
        IReadOnlyCollection<IUser> CachedUsers { get; }

        /// <summary> Gets a collection of all users in this channel. </summary>
        IAsyncEnumerable<IReadOnlyCollection<IUser>> GetUsersAsync();
        
        /// <summary> Gets a user in this channel with the provided id.</summary>
        Task<IUser> GetUserAsync(ulong id);
        IUser GetCachedUser(ulong id);
    }
}
