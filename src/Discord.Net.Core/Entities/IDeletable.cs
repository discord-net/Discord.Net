using System.Threading.Tasks;

namespace Discord
{
    /// <summary> Represents whether the object is deletable or not. </summary>
    public interface IDeletable
    {
        /// <summary> Deletes this object and all its children. </summary>
        Task DeleteAsync(RequestOptions options = null);
    }
}
