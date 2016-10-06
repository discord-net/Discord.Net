using System.Threading.Tasks;

namespace Discord
{
    public interface IGroupChannel : IMessageChannel, IPrivateChannel, IAudioChannel
    {
        /// <summary> Leaves this group. </summary>
        Task LeaveAsync(RequestOptions options = null);
    }
}