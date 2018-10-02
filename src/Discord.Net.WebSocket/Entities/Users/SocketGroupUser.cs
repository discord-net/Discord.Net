using System.Diagnostics;
using Model = Discord.API.User;

namespace Discord.WebSocket
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class SocketGroupUser : SocketUser, IGroupUser
    {
        /// <summary>
        ///     Gets the group channel of the user.
        /// </summary>
        /// <returns>
        ///     A <see cref="SocketGroupChannel" /> representing the channel of which the user belongs to.
        /// </returns>
        public SocketGroupChannel Channel { get; }
        /// <inheritdoc />
        internal override SocketGlobalUser GlobalUser { get; }

        /// <inheritdoc />
        public override bool IsBot { get { return GlobalUser.IsBot; } internal set { GlobalUser.IsBot = value; } }
        /// <inheritdoc />
        public override string Username { get { return GlobalUser.Username; } internal set { GlobalUser.Username = value; } }
        /// <inheritdoc />
        public override ushort DiscriminatorValue { get { return GlobalUser.DiscriminatorValue; } internal set { GlobalUser.DiscriminatorValue = value; } }
        /// <inheritdoc />
        public override string AvatarId { get { return GlobalUser.AvatarId; } internal set { GlobalUser.AvatarId = value; } }
        /// <inheritdoc />
        internal override SocketPresence Presence { get { return GlobalUser.Presence; } set { GlobalUser.Presence = value; } }

        /// <inheritdoc />
        public override bool IsWebhook => false;

        internal SocketGroupUser(SocketGroupChannel channel, SocketGlobalUser globalUser)
            : base(channel.Discord, globalUser.Id)
        {
            Channel = channel;
            GlobalUser = globalUser;
        }
        internal static SocketGroupUser Create(SocketGroupChannel channel, ClientState state, Model model)
        {
            var entity = new SocketGroupUser(channel, channel.Discord.GetOrCreateUser(state, model));
            entity.Update(state, model);
            return entity;
        }

        private string DebuggerDisplay => $"{Username}#{Discriminator} ({Id}{(IsBot ? ", Bot" : "")}, Group)";
        internal new SocketGroupUser Clone() => MemberwiseClone() as SocketGroupUser;

        //IVoiceState
        /// <inheritdoc />
        bool IVoiceState.IsDeafened => false;
        /// <inheritdoc />
        bool IVoiceState.IsMuted => false;
        /// <inheritdoc />
        bool IVoiceState.IsSelfDeafened => false;
        /// <inheritdoc />
        bool IVoiceState.IsSelfMuted => false;
        /// <inheritdoc />
        bool IVoiceState.IsSuppressed => false;
        /// <inheritdoc />
        IVoiceChannel IVoiceState.VoiceChannel => null;
        /// <inheritdoc />
        string IVoiceState.VoiceSessionId => null;
    }
}
