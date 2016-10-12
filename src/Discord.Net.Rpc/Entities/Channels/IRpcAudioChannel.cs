using System.Collections.Generic;

namespace Discord.Rpc
{
    public interface IRpcAudioChannel : IAudioChannel
    {
        IReadOnlyCollection<RpcVoiceState> VoiceStates { get; }
    }
}
