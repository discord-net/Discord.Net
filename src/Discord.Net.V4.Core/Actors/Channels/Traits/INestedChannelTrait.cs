namespace Discord;

public interface INestedChannelTrait
{
    IGuildChannelTrait Parent { get; }
}