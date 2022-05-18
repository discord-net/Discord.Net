using System;
using System.Diagnostics;
using Model = Discord.API.User;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a WebSocket-based group user.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class SocketGroupUser : SocketUser, IGroupUser
    {
        #region SocketGroupUser
        /// <summary>
        ///     Gets the group channel of the user.
        /// </summary>
        /// <returns>
        ///     A <see cref="SocketGroupChannel" /> representing the channel of which the user belongs to.
        /// </returns>
        public SocketGroupChannel Channel { get; }

        /// <inheritdoc />
        public override bool IsWebhook => false;

        internal SocketGroupUser(SocketGroupChannel channel, ulong userId)
            : base(channel.Discord, userId)
        {
            Channel = channel;
        }
        internal static SocketGroupUser Create(SocketGroupChannel channel, Model model)
        {
            var entity = new SocketGroupUser(channel, model.Id);
            entity.Update(model);
            return entity;
        }

        private string DebuggerDisplay => $"{Username}#{Discriminator} ({Id}{(IsBot ? ", Bot" : "")}, Group)";
        internal new SocketGroupUser Clone() => MemberwiseClone() as SocketGroupUser;
        public override void Dispose()
        {
            GC.SuppressFinalize(this);

            if (GlobalUser.IsValueCreated)
                GlobalUser.Value.Dispose();
        }
        ~SocketGroupUser() => Dispose();

        #endregion

        #region IVoiceState
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
        /// <inheritdoc />
        bool IVoiceState.IsStreaming => false;
        /// <inheritdoc />
        bool IVoiceState.IsVideoing => false;
        /// <inheritdoc />
        DateTimeOffset? IVoiceState.RequestToSpeakTimestamp => null;
        #endregion
    }
}
