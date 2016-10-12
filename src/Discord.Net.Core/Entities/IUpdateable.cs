using System.Threading.Tasks;

namespace Discord
{
    public interface IUpdateable
    {
        /// <summary> Updates this object's properties with its current state. </summary>
        Task UpdateAsync(RequestOptions options = null);
    }
}
