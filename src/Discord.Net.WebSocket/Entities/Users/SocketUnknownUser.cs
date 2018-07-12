using System;
using System.Diagnostics;
using Model = Discord.API.User;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a WebSocket-based user that is yet to be recognized by the client.
    /// </summary>
    /// <remarks>
    ///     A user may not be recognized due to the user missing from the cache or failed to be recognized properly.
    /// </remarks>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketUnknownUser : SocketUser
    {
        /// <inheritdoc />
        public override string Username { get; internal set; }
        /// <inheritdoc />
        public override ushort DiscriminatorValue { get; internal set; }
        /// <inheritdoc />
        public override string AvatarId { get; internal set; }
        /// <inheritdoc />
        public override bool IsBot { get; internal set; }
        
        /// <inheritdoc />
        public override bool IsWebhook => false;

        internal override SocketPresence Presence { get { return new SocketPresence(UserStatus.Offline, null); } set { } }
        /// <inheritdoc />
        /// <exception cref="NotSupportedException">This field is not supported for an unknown user.</exception>
        internal override SocketGlobalUser GlobalUser =>
            throw new NotSupportedException();

        internal SocketUnknownUser(DiscordSocketClient discord, ulong id)
            : base(discord, id)
        {
        }
        internal static SocketUnknownUser Create(DiscordSocketClient discord, ClientState state, Model model)
        {
            var entity = new SocketUnknownUser(discord, model.Id);
            entity.Update(state, model);
            return entity;
        }

        private string DebuggerDisplay => $"{Username}#{Discriminator} ({Id}{(IsBot ? ", Bot" : "")}, Unknown)";
        internal new SocketUnknownUser Clone() => MemberwiseClone() as SocketUnknownUser;
    }
}
