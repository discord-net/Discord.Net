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
        /// <summary> Gets the guild this invite is linked to. </summary>
        IGuild Guild { get; }
        /// <summary> Gets the id of the guild this invite is linked to. </summary>
        ulong GuildId { get; }

        /// <summary> Accepts this invite and joins the target guild. This will fail on bot accounts. </summary>
        Task AcceptAsync(RequestOptions options = null);
    }
}
