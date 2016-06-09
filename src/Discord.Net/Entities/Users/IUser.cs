using System.Threading.Tasks;

namespace Discord
{
    public interface IUser : ISnowflakeEntity, IMentionable, IPresence
    {
        /// <summary> Gets the url to this user's avatar. </summary>
        string AvatarUrl { get; }
        /// <summary> Gets the per-username unique id for this user. </summary>
        string Discriminator { get; }
        /// <summary> Returns true if this user is a bot account. </summary>
        bool IsBot { get; }
        /// <summary> Gets the username for this user. </summary>
        string Username { get; }

        //TODO: CreateDMChannel is a candidate to move to IGuildUser, and User made a common class, depending on next friends list update
        /// <summary> Returns a private message channel to this user, creating one if it does not already exist. </summary>
        Task<IDMChannel> CreateDMChannelAsync();
    }
}
