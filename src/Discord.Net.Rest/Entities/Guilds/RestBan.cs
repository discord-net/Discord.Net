using System.Diagnostics;
using Model = Discord.API.Ban;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a REST-based ban object.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestBan : IBan
    {
        /// <summary>
        ///     Gets the banned user.
        /// </summary>
        /// <returns>
        ///     A generic <see cref="RestUser"/> object that was banned.
        /// </returns>
        public RestUser User { get; }
        /// <inheritdoc />
        public string Reason { get; }

        internal RestBan(RestUser user, string reason)
        {
            User = user;
            Reason = reason;
        }
        internal static RestBan Create(BaseDiscordClient client, Model model)
        {
            return new RestBan(RestUser.Create(client, model.User), model.Reason);
        }

        /// <summary>
        ///     Gets the name of the banned user.
        /// </summary>
        /// <returns>
        ///     A string containing the name of the user that was banned.
        /// </returns>
        public override string ToString() => User.ToString();
        private string DebuggerDisplay => $"{User}: {Reason}";

        //IBan
        /// <inheritdoc />
        IUser IBan.User => User;
    }
}
