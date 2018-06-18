using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    /// A type of guild channel that can be nested within a category.
    /// Contains a CategoryId that is set to the parent category, if it is set.
    /// </summary>
    public interface INestedChannel : IGuildChannel
    {
        /// <summary> Gets the parentid (category) of this channel in the guild's channel list. </summary>
        ulong? CategoryId { get; }
        /// <summary> Gets the parent channel (category) of this channel, if it is set. If unset, returns null.</summary>
        Task<ICategoryChannel> GetCategoryAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
    }
}
