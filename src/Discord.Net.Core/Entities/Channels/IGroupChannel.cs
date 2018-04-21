using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a private generic group channel.
    /// </summary>
    public interface IGroupChannel : IMessageChannel, IPrivateChannel, IAudioChannel
    {
        /// <summary>
        ///     Leaves this group.
        /// </summary>
        Task LeaveAsync(RequestOptions options = null);
    }
}
