using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents whether the object is updatable or not.
    /// </summary>
    public interface IUpdateable
    {
        /// <summary>
        ///     Updates this object's properties with its current state.
        /// </summary>
        Task UpdateAsync(RequestOptions options = null);
    }
}
