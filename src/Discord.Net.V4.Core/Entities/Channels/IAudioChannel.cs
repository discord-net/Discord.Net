namespace Discord;

public interface IAudioChannel : IChannel, IModifyable<AudioChannelProperties>
{
    string? RTCRegion { get; }
}
