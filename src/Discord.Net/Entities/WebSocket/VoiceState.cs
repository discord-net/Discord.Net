using System;

namespace Discord
{
    internal struct VoiceState : IVoiceState
    {
        [Flags]
        private enum Flags : byte
        {
            None = 0x0,
            Suppressed = 0x1,
            SelfMuted = 0x2,
            SelfDeafened = 0x4,
        }

        private readonly Flags _voiceStates;
        
        public CachedVoiceChannel VoiceChannel { get; }
        public string VoiceSessionId { get; }

        public bool IsSelfMuted => (_voiceStates & Flags.SelfMuted) != 0;
        public bool IsSelfDeafened => (_voiceStates & Flags.SelfDeafened) != 0;
        public bool IsSuppressed => (_voiceStates & Flags.Suppressed) != 0;
        
        public VoiceState(CachedVoiceChannel voiceChannel, string sessionId, bool isSelfMuted, bool isSelfDeafened, bool isSuppressed)
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

        IVoiceChannel IVoiceState.VoiceChannel => VoiceChannel;
    }
}
