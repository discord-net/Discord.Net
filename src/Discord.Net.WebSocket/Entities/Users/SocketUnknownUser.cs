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
        /// <inheritdoc />
        internal override LazyCached<SocketPresence> Presence { get { return new(SocketPresence.Default); } set { } }
        internal override LazyCached<SocketGlobalUser> GlobalUser { get => new(null); set { } }

        internal SocketUnknownUser(DiscordSocketClient discord, ulong id)
            : base(discord, id)
        {
        }
        internal static SocketUnknownUser Create(DiscordSocketClient discord, Model model)
        {
            var entity = new SocketUnknownUser(discord, model.Id);
            entity.Update(model);
            return entity;
        }

        public override void Dispose() { }

        private string DebuggerDisplay => $"{Username}#{Discriminator} ({Id}{(IsBot ? ", Bot" : "")}, Unknown)";
        internal new SocketUnknownUser Clone() => MemberwiseClone() as SocketUnknownUser;
    }
}
