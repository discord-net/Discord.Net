using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    public interface IGroupChannel : IMessageChannel, IPrivateChannel
    {
        ///// <summary> Adds a user to this group. </summary>
        //Task AddUserAsync(IUser user);

        //new IReadOnlyCollection<IGroupUser> CachedUsers { get; }

        /// <summary> Leaves this group. </summary>
        Task LeaveAsync();
    }
}