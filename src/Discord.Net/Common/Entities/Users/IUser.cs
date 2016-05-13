using System.Threading.Tasks;

namespace Discord
{
    public interface IUser : ISnowflakeEntity, IMentionable
    {
        /// <summary> Gets the url to this user's avatar. </summary>
        string AvatarUrl { get; }
        /// <summary> Gets the game this user is currently playing, if any. </summary>
        string CurrentGame { get; }
        /// <summary> Gets the per-username unique id for this user. </summary>
        ushort Discriminator { get; }
        /// <summary> Returns true if this user is a bot account. </summary>
        bool IsBot { get; }
        /// <summary> Gets the current status of this user. </summary>
        UserStatus Status { get; }
        /// <summary> Gets the username for this user. </summary>
        string Username { get; }

        /// <summary> Returns a private message channel to this user, creating one if it does not already exist. </summary>
        Task<IDMChannel> CreateDMChannel();
    }
}
