using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a generic group channel.
    /// </summary>
    public interface IGroupChannel : IMessageChannel, IPrivateChannel, IAudioChannel
    {
        /// <summary>
        ///     Leaves this group.
        /// </summary>
        Task LeaveAsync(RequestOptions options = null);
    }
}
