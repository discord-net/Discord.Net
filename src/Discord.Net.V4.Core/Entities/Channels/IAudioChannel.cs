namespace Discord;

public interface IAudioChannel : IAudioChannel<IAudioChannel>;
public interface IAudioChannel<out TChannel> :
    IChannel<TChannel>
    where TChannel : IAudioChannel<TChannel>
{
    string? RTCRegion { get; }
}
