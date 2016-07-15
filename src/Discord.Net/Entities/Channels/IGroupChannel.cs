using System.Threading.Tasks;

namespace Discord
{
    public interface IGroupChannel : IMessageChannel, IPrivateChannel
    {
        /// <summary> Leaves this group. </summary>
        Task LeaveAsync();
    }
}