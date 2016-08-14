using System;
using Model = Discord.API.VoiceState;

namespace Discord.WebSocket
{
    //TODO: C#7 Candidate for record type
    internal struct VoiceState : IVoiceState
    {
        [Flags]
        private enum Flags : byte
        {
            None = 0x00,
            Suppressed = 0x01,
            Muted = 0x02,
            Deafened = 0x04,
            SelfMuted = 0x08,
            SelfDeafened = 0x10,
        }

        private readonly Flags _voiceStates;
        
        public SocketVoiceChannel VoiceChannel { get; }
        public string VoiceSessionId { get; }

        public bool IsMuted => (_voiceStates & Flags.Muted) != 0;
        public bool IsDeafened => (_voiceStates & Flags.Deafened) != 0;
        public bool IsSuppressed => (_voiceStates & Flags.Suppressed) != 0;
        public bool IsSelfMuted => (_voiceStates & Flags.SelfMuted) != 0;
        public bool IsSelfDeafened => (_voiceStates & Flags.SelfDeafened) != 0;

        public VoiceState(SocketVoiceChannel voiceChannel, Model model) 
            : this(voiceChannel, model.SessionId, model.SelfMute, model.SelfDeaf, model.Suppress) { }
        public VoiceState(SocketVoiceChannel voiceChannel, string sessionId, bool isSelfMuted, bool isSelfDeafened, bool isSuppressed)
        {
            VoiceChannel = voiceChannel;
            VoiceSessionId = sessionId;

            Flags voiceStates = Flags.None;
            if (isSelfMuted)
                voiceStates |= Flags.SelfMuted;
            if (isSelfDeafened)
                voiceStates |= Flags.SelfDeafened;
            if (isSuppressed)
                voiceStates |= Flags.Suppressed;
            _voiceStates = voiceStates;
        }

        public VoiceState Clone() => this;

        IVoiceChannel IVoiceState.VoiceChannel => VoiceChannel;
    }
}
