using System.Threading.Tasks;

namespace Discord
{
    public interface IUpdateable
    {
        /// <summary> Ensures this objects's cached properties reflect its current state on the Discord server. </summary>
        Task Update();
    }
}
