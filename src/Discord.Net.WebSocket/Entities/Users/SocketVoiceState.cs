using System;
using System.Diagnostics;
using Model = Discord.API.VoiceState;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a WebSocket user's voice connection status.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct SocketVoiceState : IVoiceState
    {
        /// <summary>
        ///     Initializes a default <see cref="SocketVoiceState"/> with everything set to <c>null</c> or <c>false</c>.
        /// </summary>
        public static readonly SocketVoiceState Default = new SocketVoiceState(null, null, null, false, false, false, false, false, false, false);

        [Flags]
        private enum Flags : byte
        {
            Normal = 0x00,
            Suppressed = 0x01,
            Muted = 0x02,
            Deafened = 0x04,
            SelfMuted = 0x08,
            SelfDeafened = 0x10,
            SelfStream = 0x20,
            SelfVideo = 0x40,
        }

        private readonly Flags _voiceStates;

        /// <summary>
        ///     Gets the voice channel that the user is currently in; or <c>null</c> if none.
        /// </summary>
        public SocketVoiceChannel VoiceChannel { get; }
        /// <inheritdoc />
        public string VoiceSessionId { get; }
        /// <inheritdoc/>
        public DateTimeOffset? RequestToSpeakTimestamp { get; private set; }

        /// <inheritdoc />
        public bool IsMuted => (_voiceStates & Flags.Muted) != 0;
        /// <inheritdoc />
        public bool IsDeafened => (_voiceStates & Flags.Deafened) != 0;
        /// <inheritdoc />
        public bool IsSuppressed => (_voiceStates & Flags.Suppressed) != 0;
        /// <inheritdoc />
        public bool IsSelfMuted => (_voiceStates & Flags.SelfMuted) != 0;
        /// <inheritdoc />
        public bool IsSelfDeafened => (_voiceStates & Flags.SelfDeafened) != 0;
        /// <inheritdoc />
        public bool IsStreaming => (_voiceStates & Flags.SelfStream) != 0;
        /// <inheritdoc />
        public bool IsVideoing => (_voiceStates & Flags.SelfVideo) != 0;


        internal SocketVoiceState(SocketVoiceChannel voiceChannel, DateTimeOffset? requestToSpeak, string sessionId, bool isSelfMuted, bool isSelfDeafened, bool isMuted, bool isDeafened, bool isSuppressed, bool isStream, bool isVideo)
        {
            VoiceChannel = voiceChannel;
            VoiceSessionId = sessionId;
            RequestToSpeakTimestamp = requestToSpeak;

            Flags voiceStates = Flags.Normal;
            if (isSelfMuted)
                voiceStates |= Flags.SelfMuted;
            if (isSelfDeafened)
                voiceStates |= Flags.SelfDeafened;
            if (isMuted)
                voiceStates |= Flags.Muted;
            if (isDeafened)
                voiceStates |= Flags.Deafened;
            if (isSuppressed)
                voiceStates |= Flags.Suppressed;
            if (isStream)
                voiceStates |= Flags.SelfStream;
            if (isVideo)
                voiceStates |= Flags.SelfVideo;
            _voiceStates = voiceStates;
        }
        internal static SocketVoiceState Create(SocketVoiceChannel voiceChannel, Model model)
        {
            return new SocketVoiceState(voiceChannel, model.RequestToSpeakTimestamp.IsSpecified ? model.RequestToSpeakTimestamp.Value : null, model.SessionId, model.SelfMute, model.SelfDeaf, model.Mute, model.Deaf, model.Suppress, model.SelfStream, model.SelfVideo);
        }

        /// <summary>
        ///     Gets the name of this voice channel.
        /// </summary>
        /// <returns>
        ///     A string that resolves to name of this voice channel; otherwise "Unknown".
        /// </returns>
        public override string ToString() => VoiceChannel?.Name ?? "Unknown";
        private string DebuggerDisplay => $"{VoiceChannel?.Name ?? "Unknown"} ({_voiceStates})";
        internal SocketVoiceState Clone() => this;

        /// <inheritdoc />
        IVoiceChannel IVoiceState.VoiceChannel => VoiceChannel;
    }
}
