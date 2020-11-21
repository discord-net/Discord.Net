using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Defines whether the object is updateable or not.
    /// </summary>
    public interface IUpdateable
    {
        /// <summary>
        ///     Updates this object's properties with its current state.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        Task UpdateAsync(RequestOptions options = null);
    }
}
