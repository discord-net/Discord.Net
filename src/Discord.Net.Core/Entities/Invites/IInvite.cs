using System.Threading.Tasks;

namespace Discord
{
    public interface IInvite : IEntity<string>, IDeletable
    {
        /// <summary> Gets the unique identifier for this invite. </summary>
        string Code { get; }
        /// <summary> Gets the url used to accept this invite, using Code. </summary>
        string Url { get; }

        /// <summary> Gets the channel this invite is linked to. </summary>
        IChannel Channel { get; }
        /// <summary> Gets the id of the channel this invite is linked to. </summary>
        ulong ChannelId { get; }
        /// <summary> Gets the name of the channel this invite is linked to. </summary>
        string ChannelName { get; }

        /// <summary> Gets the guild this invite is linked to. </summary>
        IGuild Guild { get; }
        /// <summary> Gets the id of the guild this invite is linked to. </summary>
        ulong GuildId { get; }
        /// <summary> Gets the name of the guild this invite is linked to. </summary>
        string GuildName { get; }
        /// <summary> Gets the approximated count of online members in the guild. </summary>
        int? PresenceCount { get; }
        /// <summary> Gets the approximated count of total members in the guild. </summary>
        int? MemberCount { get; }
    }
}
