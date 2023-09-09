namespace Discord;

public interface IAudioChannel : IChannel, IModifyable<ModifyAudioChannelProperties>
{
    string? RTCRegion { get; }
}
