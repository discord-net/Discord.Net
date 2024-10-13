namespace Discord;

public interface IMentionedChannel
{
    IGuildChannelActor Channel { get; }
    IGuildActor Guild { get; }
    string Name { get; }
    ChannelType Type { get; }
}