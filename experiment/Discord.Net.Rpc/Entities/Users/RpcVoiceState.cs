using System;
using System.Diagnostics;
using Model = Discord.API.Rpc.ExtendedVoiceState;

namespace Discord.Rpc
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RpcVoiceState : IVoiceState
    {
        [Flags]
        private enum Flags : byte
        {
            Normal = 0x00,
            Suppressed = 0x01,
            Muted = 0x02,
            Deafened = 0x04,
            SelfMuted = 0x08,
            SelfDeafened = 0x10,
        }

        private Flags _voiceStates;

        public RpcUser User { get; }
        public string Nickname { get; private set; }
        public int Volume { get; private set; }
        public bool IsMuted2 { get; private set; }
        public Pan Pan { get; private set; }

        public bool IsMuted => (_voiceStates & Flags.Muted) != 0;
        public bool IsDeafened => (_voiceStates & Flags.Deafened) != 0;
        public bool IsSuppressed => (_voiceStates & Flags.Suppressed) != 0;
        public bool IsSelfMuted => (_voiceStates & Flags.SelfMuted) != 0;
        public bool IsSelfDeafened => (_voiceStates & Flags.SelfDeafened) != 0;

        internal RpcVoiceState(DiscordRpcClient discord, ulong userId)
        {
            User = new RpcUser(discord, userId);
        }
        internal static RpcVoiceState Create(DiscordRpcClient discord, Model model)
        {
            var entity = new RpcVoiceState(discord, model.User.Id);
            entity.Update(model);
            return entity;
        }
        internal void Update(Model model)
        {
            if (model.VoiceState.IsSpecified)
            {
                Flags voiceStates = Flags.Normal;
                if (model.VoiceState.Value.Mute)
                    voiceStates |= Flags.Muted;
                if (model.VoiceState.Value.Deaf)
                    voiceStates |= Flags.Deafened;
                if (model.VoiceState.Value.SelfMute)
                    voiceStates |= Flags.SelfMuted;
                if (model.VoiceState.Value.SelfDeaf)
                    voiceStates |= Flags.SelfDeafened;
                if (model.VoiceState.Value.Suppress)
                    voiceStates |= Flags.Suppressed;
                _voiceStates = voiceStates;
            }
            User.Update(model.User);
            if (model.Nickname.IsSpecified)
                Nickname = model.Nickname.Value;
            if (model.Volume.IsSpecified)
                Volume = model.Volume.Value;
            if (model.Mute.IsSpecified)
                IsMuted2 = model.Mute.Value;
            if (model.Pan.IsSpecified)
                Pan = Pan.Create(model.Pan.Value);
        }

        public override string ToString() => User.ToString();
        private string DebuggerDisplay => $"{User} ({_voiceStates})";

        string IVoiceState.VoiceSessionId { get { throw new NotSupportedException(); } }
        IVoiceChannel IVoiceState.VoiceChannel { get { throw new NotSupportedException(); } }
    }
}
