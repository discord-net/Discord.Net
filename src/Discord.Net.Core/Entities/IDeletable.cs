using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Determines whether the object is deletable or not.
    /// </summary>
    public interface IDeletable
    {
        /// <summary>
        ///     Deletes this object and all its children.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        Task DeleteAsync(RequestOptions options = null);
    }
}
