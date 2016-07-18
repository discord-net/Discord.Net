using System.Threading.Tasks;

namespace Discord
{
    public interface IGroupChannel : IMessageChannel, IPrivateChannel
    {
        /// <summary> Adds a user to this group. </summary>
        Task AddUserAsync(IUser user);

        /// <summary> Leaves this group. </summary>
        Task LeaveAsync();
    }
}