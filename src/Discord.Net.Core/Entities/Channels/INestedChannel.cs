using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a type of guild channel that can be nested within a category.
    /// </summary>
    public interface INestedChannel : IGuildChannel
    {
        /// <summary>
        ///     Gets the parent (category) ID of this channel in the guild's channel list.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the snowflake identifier of the parent of this channel; 
        ///     <c>null</c> if none is set.
        /// </returns>
        ulong? CategoryId { get; }
        /// <summary>
        ///     Gets the parent (category) channel of this channel.
        /// </summary>
        /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains the category channel
        ///     representing the parent of this channel; <c>null</c> if none is set.
        /// </returns>
        Task<ICategoryChannel> GetCategoryAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);

        /// <summary>
        ///     Syncs the permissions of this nested channel with its parent's.
        /// </summary>
        Task SyncPermissionsAsync(RequestOptions options = null);
    }
}
