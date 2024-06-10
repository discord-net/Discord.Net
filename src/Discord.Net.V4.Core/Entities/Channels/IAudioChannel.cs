namespace Discord;

public interface IAudioChannel : IChannel
{
    string? RTCRegion { get; }
}
