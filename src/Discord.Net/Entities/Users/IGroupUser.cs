using System.Threading.Tasks;

namespace Discord
{
    public interface IGroupUser : IUser
    {
        /// <summary> Kicks this user from this group. </summary>
        Task KickAsync();

        /// <summary> Returns a private message channel to this user, creating one if it does not already exist. </summary>
        Task<IDMChannel> CreateDMChannelAsync();
    }
}
