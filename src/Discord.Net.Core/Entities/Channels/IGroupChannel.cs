using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a generic private group channel.
    /// </summary>
    public interface IGroupChannel : IMessageChannel, IPrivateChannel, IAudioChannel
    {
        /// <summary>
        ///     Leaves this group.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous leave operation.
        /// </returns>
        Task LeaveAsync(RequestOptions options = null);
    }
}
