using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    public interface ISocketInvite
    {
        /// <summary>
        ///     Gets the unique identifier for this invite.
        /// </summary>
        /// <returns>
        ///     A string containing the invite code (e.g. <c>FTqNnyS</c>).
        /// </returns>
        string Code { get; }
        /// <summary>
        ///     Gets the URL used to accept this invite
        /// </summary>
        /// <returns>
        ///     A string containing the full invite URL (e.g. <c>https://discord.gg/FTqNnyS</c>).
        /// </returns>
        string Url { get; }

        /// <summary>
        ///     Gets the channel this invite is linked to.
        /// </summary>
        /// <returns>
        ///     A generic channel that the invite points to.
        /// </returns>
        SocketGuildChannel Channel { get; }
       
        /// <summary>
        ///     Gets the guild this invite is linked to.
        /// </summary>
        /// <returns>
        ///     A guild object representing the guild that the invite points to.
        /// </returns>
        SocketGuild Guild { get; }
    }
}
